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

        public void SendMessages(HttpContextBase context, IEnumerable<Message> messages)
        {
            context.Response.ContentType = "text/javascript";

            context.Response.Write(callback ?? "jsonpcallback");
            context.Response.Write("(");
            context.Response.Write(MessageConverter.ToJson(messages));
            context.Response.Write(");");
        }
    }
}