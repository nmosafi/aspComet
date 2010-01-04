using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IClientRepository clientRepository;
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly IClientFactory clientFactory;
        private readonly IMessageHandler metaConnectHandler;
        private readonly IMessageHandler metaDisconnectHandler;
        private readonly IMessageHandler metaHandshakeHandler;
        private readonly IMessageHandler metaSubscribeHandler;
        private readonly IMessageHandler metaUnsubscribeHandler;
        private readonly IMessageHandler swallowHandler;

        public MessageHandlerFactory(IClientRepository clientRepository, IClientIDGenerator clientIDGenerator, IClientFactory clientFactory)
        {
            this.clientRepository = clientRepository;
            this.clientIDGenerator = clientIDGenerator;
            this.clientFactory = clientFactory;

            this.metaConnectHandler = new MetaConnectHandler(this.clientRepository);
            this.metaDisconnectHandler = new MetaDisconnectHandler(this.clientRepository);
            this.metaHandshakeHandler = new MetaHandshakeHandler(this.clientIDGenerator, this.clientFactory, this.clientRepository);
            this.metaSubscribeHandler = new MetaSubscribeHandler(this.clientRepository);
            this.metaUnsubscribeHandler = new MetaUnsubscribeHandler(this.clientRepository);
            this.swallowHandler = new SwallowHandler();
        }

        public IMessageHandler GetMessageHandler(string channelName)
        {
            if (channelName == null)
            {
                return new ExceptionHandler("Empty channel field in request.");
            }

            if (channelName.StartsWith("/meta/"))
            {
                return this.GetMetaHandler(channelName);
            }

            if (channelName.StartsWith("/service/"))
            {
                return this.swallowHandler;
            }

            return new PassThruHandler(channelName, this.clientRepository.WhereSubscribedTo(channelName));
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