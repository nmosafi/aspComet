using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IMessageHandler metaConnectHandler;
        private readonly IMessageHandler metaDisconnectHandler;
        private readonly IMessageHandler metaHandshakeHandler;
        private readonly IMessageHandler metaSubscribeHandler;
        private readonly IMessageHandler metaUnsubscribeHandler;
        private readonly IMessageHandler swallowHandler;
        private readonly IMessageHandler forwardingHandler;

        public MessageHandlerFactory(IClientRepository clientRepository, IClientIDGenerator clientIDGenerator, IClientFactory clientFactory, IClientWorkflowManager clientWorkflowManager)
        {
            this.metaConnectHandler = new MetaConnectHandler(clientRepository);
            this.metaDisconnectHandler = new MetaDisconnectHandler(clientRepository);
            this.metaHandshakeHandler = new MetaHandshakeHandler(clientIDGenerator, clientFactory, clientWorkflowManager);
            this.metaSubscribeHandler = new MetaSubscribeHandler(clientRepository);
            this.metaUnsubscribeHandler = new MetaUnsubscribeHandler(clientRepository);
            this.swallowHandler = new SwallowHandler();
            this.forwardingHandler = new ForwardingHandler(clientRepository);
        }

        public IMessageHandler GetMessageHandler(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                return new ExceptionHandler("Empty channel field in request");
            }

            if (channelName.StartsWith("/meta/"))
            {
                return this.GetMetaHandler(channelName);
            }

            if (channelName.StartsWith("/service/"))
            {
                return this.swallowHandler;
            }

            return this.forwardingHandler;
        }

        private IMessageHandler GetMetaHandler(string channelName)
        {
            switch (channelName.Substring(6))
            {
                case "connect": return this.metaConnectHandler;
                case "disconnect": return this.metaDisconnectHandler;
                case "handshake": return this.metaHandshakeHandler;
                case "subscribe": return this.metaSubscribeHandler;
                case "unsubscribe": return this.metaUnsubscribeHandler;
                default: return new ExceptionHandler("Unknown meta channel.");
            }
        }

    }
}