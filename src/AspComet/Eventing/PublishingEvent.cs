using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet.Eventing
{
    public class PublishingEvent : ICancellableEvent
    {
        public bool Cancel { get; set; }
        public Message Message { get; private set; }

        public PublishingEvent(Message message)
        {
            Message = message;
        }
    }
}
