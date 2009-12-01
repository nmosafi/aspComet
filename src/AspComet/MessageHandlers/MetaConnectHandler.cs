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
            // First, check we have a client
            if (request.clientId == null || !source.ContainsClient(request.clientId))
            {
                return GetConnectResponseUnrecognisedClient(request);
            }

            Client client = source.GetClient(request.clientId);

            if (!client.IsConnected)
                shouldWait = false;

            client.NotifyConnected();
            return GetConnectResponseSucceeded(request);
        }

        private Message GetConnectResponseSucceeded(Message request)
        {
            // The connect response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and

            Dictionary<string, int> connectAdvice = new Dictionary<string, int>();
            connectAdvice["timeout"] = CometHttpHandler.LongPollDuration;
            return new Message
            {
                id = request.id,
                channel = this.ChannelName,
                successful = true,
                clientId = request.clientId,
                connectionType = "long-polling",
                advice = connectAdvice,
            };
        }

        private Message GetConnectResponseUnrecognisedClient(Message request)
        {
            // The connect failed response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_71

            Dictionary<string, string> connectAdvice = new Dictionary<string, string>();
            connectAdvice["reconnect"] = "handshake";
            return new Message
            {
                id = request.id,
                channel = this.ChannelName,
                successful = false,
                clientId = request.clientId,
                connectionType = "long-polling",
                error = "clientId not recognised",
                advice = connectAdvice,
            };
        }

    }
}