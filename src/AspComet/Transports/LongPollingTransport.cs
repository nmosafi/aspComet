using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Practices.ServiceLocation;

namespace AspComet.Transports
{
    public class LongPollingTransport : ITransport
    {
        private readonly IMessageConverter messageConverter;
        public static LongPollingTransport Instance = new LongPollingTransport();

        public LongPollingTransport()
        {
            this.messageConverter = ServiceLocator.Current.GetInstance<IMessageConverter>();
        }

        public void SendMessages(HttpResponseBase response, IEnumerable<Message> messages)
        {
            response.ContentType = "text/json";
            response.Write(messageConverter.ToJson(messages));
        }
    }
}