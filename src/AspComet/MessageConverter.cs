using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AspComet
{
    public class MessageConverter
    {
        public static Message[] FromJson(HttpRequest request, string requestKey)
        {
            string json = request[requestKey];
            if (string.IsNullOrEmpty(json))
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                    json = reader.ReadToEnd();
            }
            using (Stream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (Message[]) new DataContractJsonSerializer(typeof(Message[])).ReadObject(stream);
            }
        }

        public static string ToJson<TModel>(TModel model)
        {
            return Regex.Replace(Serialize(model), @"""[^""]*"":null,", string.Empty);
        }

        private static string Serialize<TModel>(TModel model)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TModel));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, model);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }
    }
}