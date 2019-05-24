using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AspComet.Transports
{
    public class CallbackPollingTransport : ITransport
    {
        private readonly string callback;

        public CallbackPollingTransport(string callback)
        {
            this.callback = callback;
        }

        public void SendMessages(HttpResponse response, IEnumerable<Message> messages)
        {
            string messageAsJson = MessageConverter.ToJson(messages);
            string functionName = callback ?? "jsonpcallback";
            string functionCall = string.Format("{0} ( {1} )", functionName, messageAsJson);

            response.ContentType = "text/javascript";
            response.WriteAsync(functionCall);
        }
    }
}