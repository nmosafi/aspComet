using System;
using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class CallbackPollingTransport : ITransport
    {
        private readonly string callback;

        public CallbackPollingTransport(string callback)
        {
            this.callback = callback;
        }

        public void SendMessages(HttpResponseBase response, IEnumerable<Message> messages)
        {
            response.ContentType = "text/javascript";

            response.Write(callback ?? "jsonpcallback");
            response.Write("(");
            response.Write(MessageConverter.ToJson(messages));
            response.Write(");");
        }
    }
}