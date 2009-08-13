using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet.Eventing
{
    public class HandshakeEvent : IEvent
    {
        public bool Cancel { get; set; }
        public IClient Client { get; private set; }

        public HandshakeEvent(IClient client)
        {
            Client = client;
        }
    }
}
