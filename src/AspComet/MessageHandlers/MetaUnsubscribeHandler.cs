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
			string error;
            bool wasUnsubscribed = UnsubscribeClientAndPublishEvent(request.clientId, request.subscription, out error);

            return new MessageHandlerResult
            {
                Message = new Message
                {
                    id = request.id,
                    channel = request.channel,
                    successful = wasUnsubscribed,
                    clientId = request.clientId,
                    subscription = request.subscription,
					error = error
                },
                CanTreatAsLongPoll = false
            };
        }

        private bool UnsubscribeClientAndPublishEvent(string clientId, string subscription, out string error)
        {
			error = null;

            IClient client = clientRepository.GetByID(clientId);

			if (client == null)
			{
				error = string.Format("402:{0}:Unknown Client ID", clientId);
				return false;
			}

			if(!client.IsSubscribedTo(subscription))
				return false;

            client.UnsubscribeFrom(subscription);

            UnsubscribedEvent e = new UnsubscribedEvent(client, subscription);
            EventHub.Publish(e);

            return true;
        }
    }
}