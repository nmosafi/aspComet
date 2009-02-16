namespace AspComet.MessageHandlers
{
    public class MetaConnectHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/connect"; }
        }

        public Message HandleMessage(MessageBroker source, Message request)
        {
            Client client = source.GetClient(request.ClientID);

            return new Message
                       {
                           Channel = this.ChannelName,
                           Successful = true,
                           ClientID = client.ID,
                           ConnectionType = "long-polling"
                       };
        }
    }
}