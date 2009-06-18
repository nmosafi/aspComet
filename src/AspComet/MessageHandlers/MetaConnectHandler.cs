using System;

namespace AspComet.MessageHandlers
{
    public class MetaConnectHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/connect"; }
        }

        public bool ShouldWait
        {
            get { return true; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client client = source.GetClient(request.clientId);

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = true,
                           clientId = client.ID,
                           connectionType = "long-polling"
                       };
        }
    }
}