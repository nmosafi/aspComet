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
            string messageAsJson = MessageConverter.ToJson(messages);
            string functionName = callback ?? "jsonpcallback";
            string functionCall = string.Format("{0} ( {1} )", functionName, messageAsJson);

            response.ContentType = "text/javascript";
            response.Write(functionCall);
        }
    }
}