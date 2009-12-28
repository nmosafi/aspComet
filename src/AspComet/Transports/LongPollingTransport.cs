using System;
using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class LongPollingTransport : ITransport
    {
        public static LongPollingTransport Instance = new LongPollingTransport();

        public void SendMessages(HttpResponseBase response, IEnumerable<Message> messages)
        {
            response.ContentType = "text/json";
            response.Write(MessageConverter.ToJson(messages));
        }
    }
}