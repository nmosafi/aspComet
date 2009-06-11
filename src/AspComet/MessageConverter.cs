using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace AspComet
{
    public class MessageConverter
    {
        static readonly JavaScriptSerializer SERIALIZER = new JavaScriptSerializer();
        static readonly Regex REGEX_ARRAY = new Regex(@"^\s*\[", RegexOptions.Compiled);
        static readonly Regex REGEX_NULL = new Regex(@"(""[^""]+"":null,)|(,""[^""]+"":null)", RegexOptions.Compiled);

        public static Message[] FromJson(HttpRequest request)
        {
            // Get the "message" parameter from the post collection, as specified in Bayeux sec. 3.4.
            string json = request.Form["message"];

            // Dojo seems to just send the message as the body, without any name.
            if (json == null)
                using (var reader = new StreamReader(request.InputStream))
                    json = reader.ReadToEnd();

            // If the message starts with a [ read as an array - otherwise read into single field array.
            return REGEX_ARRAY.IsMatch(json) ? SERIALIZER.Deserialize<Message[]>(json)
                : new[] { SERIALIZER.Deserialize<Message>(json) };
        }

        public static string ToJson<TModel>(TModel model)
        {
            string txt = SERIALIZER.Serialize(model);
            return REGEX_NULL.Replace(txt, string.Empty);
        }
    }
}