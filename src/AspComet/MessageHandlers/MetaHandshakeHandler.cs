namespace AspComet.MessageHandlers
{
    public class MetaHandshakeHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/handshake"; }
        }

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client Client = source.CreateClient();

            return new Message
                       {
                           Channel = this.ChannelName,
                           Version = "1.0",
                           SupportedConnectionTypes = new[] { "long-polling" },
                           ClientID = Client.ID,
                           Successful = true
                       };
        }
    }
}