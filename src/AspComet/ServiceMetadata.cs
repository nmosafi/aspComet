using System;
using System.Collections.Generic;

namespace AspComet
{
    /// <summary>
    ///     Describes the services which need to be instantiated for AspComet
    /// </summary>
    public class ServiceMetadata
    {
        public Type ServiceType { get; private set; }
        public Type ActualType { get; private set; }
        public bool IsPerRequest { get; private set; }

        private ServiceMetadata()
        {
        }

        public static IEnumerable<ServiceMetadata> GetMinimumSet()
        {
            yield return new ServiceMetadata { ServiceType = typeof(IClientRepository), ActualType = typeof(InMemoryClientRepository) };
            yield return new ServiceMetadata { ServiceType = typeof(IClientIDGenerator), ActualType = typeof(RngUniqueClientIDGenerator) };
            yield return new ServiceMetadata { ServiceType = typeof(IClientFactory), ActualType = typeof(ClientFactory) };
            yield return new ServiceMetadata { ServiceType = typeof(IMessageHandlerFactory), ActualType = typeof(MessageHandlerFactory) };
            yield return new ServiceMetadata { ServiceType = typeof(IMessagesProcessor), ActualType = typeof(MessagesProcessor), IsPerRequest = true };
            yield return new ServiceMetadata { ServiceType = typeof(IMessageBus), ActualType = typeof(MessageBus) };
            yield return new ServiceMetadata { ServiceType = typeof(IClientWorkflowManager), ActualType = typeof(ClientWorkflowManager) };
        }
    }
}