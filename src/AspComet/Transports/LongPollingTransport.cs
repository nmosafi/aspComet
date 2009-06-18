using System;
using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class LongPollingTransport : ITransport
    {
        public void SendMessages(HttpContextBase context, IEnumerable<Message> messages)
        {
            context.Response.ContentType = "text/json";
            context.Response.Write(MessageConverter.ToJson(messages));
        }
    }
}