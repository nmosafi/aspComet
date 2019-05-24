using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class LongPollingTransport : ITransport
    {
        public static LongPollingTransport Instance = new LongPollingTransport();

        public void SendMessages(HttpResponse response, IEnumerable<Message> messages)
        {
            response.ContentType = "text/json";
            response.WriteAsync(MessageConverter.ToJson(messages));
        }
    }
}