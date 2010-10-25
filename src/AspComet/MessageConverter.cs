using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace AspComet
{
    public class MessageConverter: IMessageConverter
    {
        private readonly IMessageSerializer Serializer;
        private static readonly Regex ArrayRegex = new Regex(@"^\s*\[", RegexOptions.Compiled);

        public MessageConverter(IMessageSerializer serializer)
        {
            Serializer = serializer;
        }

        public Message[] FromJson(HttpRequestBase request)
        {
            // Get the "message" parameter from the post collection, as specified in Bayeux sec. 3.4.
            // Dojo seems to just send the message as the body, without any name, so cater for that too.
            string json = GetJsonFromMessageFieldIn(request) ?? GetJsonFromBodyOf(request);

            // If the message starts with a '[' read as an array - otherwise read into single field array.
            return ArrayRegex.IsMatch(json) ? Serializer.Deserialize<Message[]>(json)
                                            : new[] { Serializer.Deserialize<Message>(json) };
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

        public string ToJson<TModel>(TModel model)
        {
            return Serializer.Serialize(model);
        }
    }
}