using System;
using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaUnsubscribeHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/unsubscribe"; }
        }

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client client = source.GetClient(request.clientId);
            var e = new UnsubscribedEvent(client, this.ChannelName);
            EventHub.Publish(e);
            client.UnsubscribeFrom(request.subscription);

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = true,
                           clientId = client.ID,
                           subscription = request.subscription
                       };
        }
    }
}