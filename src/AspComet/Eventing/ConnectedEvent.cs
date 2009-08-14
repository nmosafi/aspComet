using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet.Eventing
{
    public class ConnectedEvent : IEvent
    {
        public IClient Client { get; private set; }

        public ConnectedEvent(IClient client)
        {
            Client = client;
        }
    }
}
