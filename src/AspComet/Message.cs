using System;
using System.Collections.Generic;

namespace AspComet
{
    public class Message
    {
        public string channel { get; set; }
        public string clientId { get; set; }
        public object data { get; set; }
        public string version { get; set; }
        public string minimumVersion { get; set; }
        public string[] supportedConnectionTypes { get; set; }
        public string advice { get; set; }
        public string connectionType { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public bool? successful { get; set; }
        public string subscription { get; set; }
        public string error { get; set; }
        public string ext { get; set; }

        public T GetData<T>(string key)
        {
            var dict = (Dictionary<string, object>)data;
            return (T)dict[key];
        }
    }
}