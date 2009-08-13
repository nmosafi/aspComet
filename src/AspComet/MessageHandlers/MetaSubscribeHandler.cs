using System;
using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaSubscribeHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/subscribe"; }
        }

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client client = source.GetClient(request.clientId);
            client.SubscribeTo(request.subscription);

            var e = new SubscribedEvent(client, request.channel);
            EventHub.Publish(e); // TODO handle e.Cancel == false

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful =  true,
                           clientId = client.ID,
                           subscription = request.subscription
                       };
        }
    }
}