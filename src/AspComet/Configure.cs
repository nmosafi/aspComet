namespace AspComet
{
    public static class Configuration
    {
        internal static IMessageBus MessageBus { get; private set; }

        public static class InitialiseHttpHandler
        {
            public static void WithMessageBus(IMessageBus messageBus)
            {
                MessageBus = messageBus;
            }

            public static void WithTheDefaultConfiguration()
            {
                IClientRepository clientRepository = new InMemoryClientRepository();
                IClientIDGenerator clientIDGenerator = new RngUniqueClientIDGenerator(clientRepository);
                IClientFactory clientFactory = new ClientFactory();
                IMessageHandlerFactory messageHandlerFactory = new MessageHandlerFactory(clientRepository, clientIDGenerator, clientFactory);

                MessageBus = new MessageBus(clientRepository, () => new MessagesProcessor(messageHandlerFactory));
            }
        }
    }
}
