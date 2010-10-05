using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaUnsubscribeHandler : IMessageHandler
    {
        private readonly IClientRepository clientRepository;

        public MetaUnsubscribeHandler(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            UnsubscribeClientAndPublishEvent(request.clientId, request.subscription);

            return new MessageHandlerResult
            {
                Message = new Message
                {
                    id = request.id,
                    channel = request.channel,
                    successful = true,
                    clientId = request.clientId,
                    subscription = request.subscription
                },
                CanTreatAsLongPoll = false
            };
        }

        private void UnsubscribeClientAndPublishEvent(string clientId, string subscription)
        {
            IClient client = this.clientRepository.GetByID(clientId);

			if (client == null)
				return;

            client.UnsubscribeFrom(subscription);

            UnsubscribedEvent e = new UnsubscribedEvent(client, subscription);
            EventHub.Publish(e);
        }
    }
}