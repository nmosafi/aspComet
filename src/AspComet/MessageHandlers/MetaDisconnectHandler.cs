using System;
using AspComet.Eventing;

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
            Client client = source.GetClient(request.clientId);
            var e = new DisconnectedEvent(client);
            EventHub.Publish(e);

            source.RemoveClient(client.ID);

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