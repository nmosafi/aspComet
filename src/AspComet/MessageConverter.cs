using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using System.Reflection;

namespace AspComet
{
    public static class MessageConverter 
    {
        public static Func<ISerializer> Serializer = () => new DefaultSerializer();

        private static readonly Regex ArrayRegex = new Regex(@"^\s*\[", RegexOptions.Compiled);
        private static readonly Regex NullRegex = new Regex(CreateNullRegexString(), RegexOptions.Compiled);

        public static Message[] FromJson(HttpRequestBase request)
        {
            // Get the "message" parameter from the post collection, as specified in Bayeux sec. 3.4.
            // Dojo seems to just send the message as the body, without any name, so cater for that too.
            string json = GetJsonFromMessageFieldIn(request) ?? GetJsonFromBodyOf(request);

            // If the message starts with a '[' read as an array - otherwise read into single field array.
            return ArrayRegex.IsMatch(json) ? Serializer().Deserialize<Message[]>(json)
                                            : new[] { Serializer().Deserialize<Message>(json) };
        }

        private static string GetJsonFromMessageFieldIn(HttpRequestBase request)
        {
            return request.Form["message"];
        }

        private static string GetJsonFromBodyOf(HttpRequestBase request)
        {
            using (var reader = new StreamReader(request.InputStream))
                return reader.ReadToEnd();
        }

        public static string ToJson<TModel>(TModel model)
        {
            string txt = Serializer().Serialize(model);
            return NullRegex.Replace(txt, string.Empty);
        }

        /// <summary>
        /// Creates a Regex which will strip out Nulls from the top level fields of the Message
        /// </summary>
        private static string CreateNullRegexString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            PropertyInfo[] properties = typeof(Message).GetProperties();
            for (int index = 0; index < properties.Length - 1; index++)
            {
                stringBuilder.AppendFormat(@"(""{0}"":null,)|(,""{0}"":null)|", properties[index].Name);
            }
            stringBuilder.AppendFormat(@"(""{0}"":null)|(,""{0}"":null)", properties[properties.Length - 1].Name);

            return stringBuilder.ToString();
        }

        public static void ResetDefaultSerializer()
        {
            Serializer = () => new DefaultSerializer();
        }
    }
}