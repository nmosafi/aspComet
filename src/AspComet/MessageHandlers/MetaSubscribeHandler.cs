using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaSubscribeHandler : IMessageHandler
    {
        private readonly IClientRepository clientRepository;

        public MetaSubscribeHandler(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            IClient client = clientRepository.GetByID(request.clientId);

			if (client == null)
			{
				return new MessageHandlerResult { Message = GetUnrecognisedClientResponse(request), CanTreatAsLongPoll = false };
			}

            ICancellableEvent subscribingEvent = PublishSubscribingEvent(request, client);

            if (subscribingEvent.Cancel)
            {   
                Message subscriptionFailedResponse = GetSubscriptionFailedResponse(request, subscribingEvent.CancellationReason);
                return new MessageHandlerResult { Message = subscriptionFailedResponse, CanTreatAsLongPoll = false };
            }

            client.SubscribeTo(request.subscription);

            PublishSubscribedEvent(request, client);

            return new MessageHandlerResult { Message = GetSubscriptionSucceededResponse(request), CanTreatAsLongPoll = false };
        }

        private static void PublishSubscribedEvent(Message request, IClient client)
        {
            SubscribedEvent subscribedEvent = new SubscribedEvent(client, request.subscription);
            EventHub.Publish(subscribedEvent);
        }

        private static ICancellableEvent PublishSubscribingEvent(Message request, IClient client)
        {
            SubscribingEvent subscribingEvent = new SubscribingEvent(client, request.subscription);
            EventHub.Publish(subscribingEvent);
            return subscribingEvent;
        }

        private static Message GetSubscriptionSucceededResponse(Message request)
        {
            return new Message
            {
                id = request.id,
                channel = request.channel,
                successful =  true,
                clientId = request.clientId,
                subscription = request.subscription
            };
        }

        private static Message GetSubscriptionFailedResponse(Message request, string cancellationReason)
        {
            // The subscription failed response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_44

            return new Message
                       {
                           id = request.id,
                           channel = request.channel,
                           successful = false,
                           clientId = request.clientId,
                           subscription = request.subscription,
                           error = string.Format("403:{0},{1}:{2}", request.clientId, request.channel, cancellationReason)
                       };
        }

		private static Message GetUnrecognisedClientResponse(Message request)
		{
			// The subscription failed response is documented at
			// http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_44

			Message response = new Message
			{
				id = request.id,
				channel = request.channel,
				successful = false,
				clientId = request.clientId,
				error = string.Format("402:{0}:Unknown Client ID", request.clientId)
			};

			return response;
		}
    }
}