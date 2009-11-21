using AspComet.Eventing;

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
            Client client = source.CreateClient();

            var e = new ConnectedEvent(client);
            EventHub.Publish(e); // TODO handle e.Cancel == false

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           version = "1.0",
                           supportedConnectionTypes = new[] { "long-polling" },
                           clientId = client.ID,
                           successful = true
                       };
        }
    }
}