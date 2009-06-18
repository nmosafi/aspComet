using System;

namespace AspComet.MessageHandlers
{
    public class MetaDisconnectHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/disconnect"; }
        }

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            source.RemoveClient(request.clientId);

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = true,
                           clientId = request.clientId
                       };
        }
    }
}