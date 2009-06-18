using System;

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