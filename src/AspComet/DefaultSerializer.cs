using System.Web.Script.Serialization;

namespace AspComet
{
    public class DefaultSerializer : ISerializer
    {
        readonly JavaScriptSerializer _serializer;

        public DefaultSerializer()
        {
            _serializer = new JavaScriptSerializer();
        }

        public string Serialize(object obj)
        {
            return _serializer.Serialize(obj);
        }

        public T Deserialize<T>(string json)
        {
            return _serializer.Deserialize<T>(json);
        }
    }
}