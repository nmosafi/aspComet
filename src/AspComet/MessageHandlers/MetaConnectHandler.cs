using System.Collections.Generic;

namespace AspComet.MessageHandlers
{
    public class MetaConnectHandler : IMessageHandler
    {
        private bool shouldWait = true;

        public string ChannelName
        {
            get { return "/meta/connect"; }
        }

        public bool ShouldWait
        {
            get { return shouldWait; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client client = source.GetClient(request.clientId);

            if (!client.IsConnected)
                shouldWait = false;

            client.NotifyConnected();

            Dictionary<string, object> advice = new Dictionary<string, object>();
            advice["timeout"] = CometHttpHandler.LongPollDuration;

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = true,
                           clientId = client.ID,
                           connectionType = "long-polling",
                           advice = advice
                       };
        }
    }
}