using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

namespace AspComet
{
    public static class Setup
    {
        public static class AspComet
        {
            public static void InANonExtensibleAndNonConfigurableManner()
            {
                IClientRepository clientRepository = new InMemoryClientRepository();
                IClientIDGenerator clientIDGenerator = new RngUniqueClientIDGenerator(clientRepository);
                IClientFactory clientFactory = new ClientFactory();
                IClientWorkflowManager clientWorkflowManager = new ClientWorkflowManager(clientRepository);
                IMessageHandlerFactory messageHandlerFactory = new MessageHandlerFactory(clientRepository, clientIDGenerator, clientFactory, clientWorkflowManager);
                IMessageBus messageBus = new MessageBus(clientRepository, () => new MessagesProcessor(messageHandlerFactory));

                ServiceLocator.SetLocatorProvider(() => new DummyServiceLocator(messageBus));
            }

            private class DummyServiceLocator : IServiceLocator
            {
                private readonly IMessageBus messageBus;

                public DummyServiceLocator(IMessageBus messageBus)
                {
                    this.messageBus = messageBus;
                }

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(IMessageBus))
                    {
                        return messageBus;
                    }
                    throw new Exception("DummyServiceLocator does not support retrieving anything but the message bus");
                }

                public object GetInstance(Type serviceType)
                {
                    if (serviceType == typeof(IMessageBus))
                    {
                        return messageBus;
                    }
                    throw new Exception("DummyServiceLocator does not support retrieving anything but the message bus");
                }

                public object GetInstance(Type serviceType, string key)
                {
                    if (serviceType == typeof(IMessageBus))
                    {
                        return messageBus;
                    }
                    throw new Exception("DummyServiceLocator does not support retrieving anything but the message bus");
                }

                public IEnumerable<object> GetAllInstances(Type serviceType)
                {
                    if (serviceType == typeof(IMessageBus))
                    {
                        yield return messageBus;
                    }
                    throw new Exception("DummyServiceLocator does not support retrieving anything but the message bus");
                }

                public TService GetInstance<TService>()
                {
                    return (TService) GetInstance(typeof(TService));
                }

                public TService GetInstance<TService>(string key)
                {
                    return (TService) GetInstance(typeof(TService), key);
                }

                public IEnumerable<TService> GetAllInstances<TService>()
                {
                    return GetAllInstances(typeof(TService)).Cast<TService>();
                }
            }
        }
    }
}
