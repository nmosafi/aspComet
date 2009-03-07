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

        public Message HandleMessage(MessageBroker source, Message request)
        {
            Client client = source.GetClient(request.ClientID);
            client.SubscribeTo(request.Subscription);

            return new Message
                       {
                           Channel = this.ChannelName,
                           Successful =  true,
                           ClientID = client.ID,
                           Subscription = request.Subscription
                       };
        }
    }
}