using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace AspComet
{
    public class MessageConverter
    {
        public static Message FromJson(HttpRequest request, string requestKey)
        {
            string json = request[requestKey];
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            using (Stream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (Message) new DataContractJsonSerializer(typeof(Message)).ReadObject(stream);
            }
        }

        public static string ToJson<TModel>(TModel model)
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