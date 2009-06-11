using System;
using System.Collections.Generic;
using System.Text;

namespace AspComet.MessageHandlers
{
    public class PassThruHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool ShouldWait
        {
            get { throw new System.NotImplementedException(); }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            throw new System.NotImplementedException();
        }
    }
}
