using System.Collections.Generic;
using System.Web;
using Microsoft.Practices.ServiceLocation;

namespace AspComet.Transports
{
    public class CallbackPollingTransport : ITransport
    {
        private readonly IMessageConverter messageConverter;
        private readonly string callback;

        public CallbackPollingTransport(string callback)
        {
            this.messageConverter = ServiceLocator.Current.GetInstance<IMessageConverter>();
            this.callback = callback;
        }

        public void SendMessages(HttpResponseBase response, IEnumerable<Message> messages)
        {
            string messageAsJson = messageConverter.ToJson(messages);
            string functionName = callback ?? "jsonpcallback";
            string functionCall = string.Format("{0} ( {1} )", functionName, messageAsJson);

            response.ContentType = "text/javascript";
            response.Write(functionCall);
        }
    }
}