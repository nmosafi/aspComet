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
            bool wasUnsubscribed = UnsubscribeClientAndPublishEvent(request.clientId, request.subscription);

            return new MessageHandlerResult
            {
                Message = new Message
                {
                    id = request.id,
                    channel = request.channel,
                    successful = wasUnsubscribed,
                    clientId = request.clientId,
                    subscription = request.subscription
                },
                CanTreatAsLongPoll = false
            };
        }

        private bool UnsubscribeClientAndPublishEvent(string clientId, string subscription)
        {
            IClient client = clientRepository.GetByID(clientId);

            if (client == null || !client.IsSubscribedTo(subscription))
                return false;

            client.UnsubscribeFrom(subscription);

            UnsubscribedEvent e = new UnsubscribedEvent(client, subscription);
            EventHub.Publish(e);

            return true;
        }
    }
}