using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet.Eventing
{
    public class PublishEvent : IEvent
    {
        public bool Cancel { get; set; }
        public Message Message { get; private set; }

        public PublishEvent(Message message)
        {
            Message = message;
        }
    }
}
