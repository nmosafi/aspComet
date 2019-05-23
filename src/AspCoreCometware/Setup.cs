using System;
using System.Collections.Generic;
using System.Linq;
using AspComet.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace AspComet
{
    public static class Setup
    {
        public static class AspComet
        {
            public static void WithTheDefaultServices()
            {
                DummyServiceLocator serviceLocator = new DummyServiceLocator();
                serviceLocator.ClientRepository = new InMemoryClientRepository();
                serviceLocator.ClientIDGenerator = new RngUniqueClientIDGenerator(serviceLocator.ClientRepository);
                serviceLocator.ClientFactory = new ClientFactory();
                serviceLocator.ClientWorkflowManager = new ClientWorkflowManager(serviceLocator.ClientRepository);
                serviceLocator.MessageHandlerFactory = new MessageHandlerFactory(serviceLocator.ClientRepository, serviceLocator.ClientIDGenerator, serviceLocator.ClientFactory, serviceLocator.ClientWorkflowManager);
                serviceLocator.MessageBus = new MessageBus(serviceLocator.ClientRepository, () => new MessagesProcessor(serviceLocator.MessageHandlerFactory));
            }

            private class DummyServiceLocator : IServiceProvider
            {
                public IMessageBus MessageBus { get; set; }
                public IClientRepository ClientRepository { get; set; }
                public IClientIDGenerator ClientIDGenerator { get; set; }
                public IClientFactory ClientFactory { get; set; }
                public IClientWorkflowManager ClientWorkflowManager { get; set; }
                public IMessageHandlerFactory MessageHandlerFactory { get; set; }

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(IMessageBus)) return MessageBus;
                    if (serviceType == typeof(IClientRepository)) return ClientRepository;
                    if (serviceType == typeof(IClientIDGenerator)) return ClientIDGenerator;
                    if (serviceType == typeof(IClientFactory)) return ClientFactory;
                    if (serviceType == typeof(IClientWorkflowManager)) return ClientWorkflowManager;
                    if (serviceType == typeof(IMessageHandlerFactory)) return MessageHandlerFactory;

                    throw new Exception("DummyServiceLocator does not support retrieving types of " + serviceType);
                }

                public object GetInstance(Type serviceType)
                {
                    return GetService(serviceType);
                }

                public object GetInstance(Type serviceType, string key)
                {
                    return GetService(serviceType);
                }

                public IEnumerable<object> GetAllInstances(Type serviceType)
                {
                    yield return GetService(serviceType);
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
