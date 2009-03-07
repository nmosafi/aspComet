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
            get { return true; }
        }

        public Message HandleMessage(MessageBroker source, Message request)
        {
            Client client = source.GetClient(request.ClientID);
            client.UnsubscribeFrom(request.Subscription);

            return new Message
                       {
                           Channel = this.ChannelName,
                           Successful = true,
                           ClientID = client.ID,
                           Subscription = request.Subscription
                       };
        }
    }
}