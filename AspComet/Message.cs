using System.Runtime.Serialization;

namespace AspComet
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "channel")]
        public string Channel { get; set; }

        [DataMember(Name = "clientId")]
        public string ClientID { get; set; }

        [DataMember(Name = "data")]
        public object Data { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "minimumVersion")]
        public string MinimumVersion { get; set; }

        [DataMember(Name = "supportedConnectionTypes")]
        public string[] SupportedConnectionTypes { get; set; }

        [DataMember(Name = "advice")]
        public string Advice { get; set; }

        [DataMember(Name = "connectionType")]
        public string ConnectionType { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }

        [DataMember(Name = "successful")]
        public bool Successful { get; set; }

        [DataMember(Name = "subscription")]
        public string Subscription { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "ext")]
        public string Ext { get; set; }
    }
}