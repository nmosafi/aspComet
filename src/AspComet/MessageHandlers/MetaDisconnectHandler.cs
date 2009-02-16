namespace AspComet.MessageHandlers
{
    public class MetaDisconnectHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/disconnect"; }
        }

        public Message HandleMessage(MessageBroker source, Message request)
        {
            source.RemoveClient(request.ClientID);

            return new Message
                       {
                           Channel = this.ChannelName,
                           Successful = true,
                           ClientID = request.ClientID
                       };
        }
    }
}