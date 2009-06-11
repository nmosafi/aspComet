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
                           channel = this.ChannelName,
                           version = "1.0",
                           supportedConnectionTypes = new[] { "long-polling" },
                           clientId = Client.ID,
                           successful = true
                       };
        }
    }
}