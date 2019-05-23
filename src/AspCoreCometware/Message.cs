// ReSharper disable InconsistentNaming
// We use lower case property names because we are using the JSON serializer which doesn't
// provide a way to attribute classes

namespace AspComet
{
    /// <summary>
    ///     A bauyeux message
    /// </summary>
    public class Message
    {
        public string channel { get; set; }
        public string clientId { get; set; }
        public object data { get; set; }
        public string version { get; set; }
        public string minimumVersion { get; set; }
        public string[] supportedConnectionTypes { get; set; }
        public object advice { get; set; }
        public string connectionType { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public bool? successful { get; set; }
        public string subscription { get; set; }
        public string error { get; set; }
        public object ext { get; set; }
    }
}