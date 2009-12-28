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

        public string ChannelName
        {
            get { return "/meta/subscribe"; }
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            Client client = clientRepository.GetByID(request.clientId);

            ICancellableEvent subscribingEvent = PubishSubscribingEvent(request, client);

            if (subscribingEvent.Cancel)
            {
                return new MessageHandlerResult
                {
                    Message = GetSubscriptionFailedResponse(request, client, subscribingEvent.CancellationReason),
                    ShouldWait = false
                };
            }

            client.SubscribeTo(request.subscription);

            PublishSubscribedEvent(request, client);

            return new MessageHandlerResult { Message = GetSubscriptionSucceededResponse(request, client), ShouldWait = false };
        }

        private static void PublishSubscribedEvent(Message request, IClient client)
        {
            SubscribedEvent subscribedEvent = new SubscribedEvent(client, request.subscription);
            EventHub.Publish(subscribedEvent);
        }

        private static ICancellableEvent PubishSubscribingEvent(Message request, IClient client)
        {
            ICancellableEvent subscribingEvent = new SubscribingEvent(client, request.subscription);
            EventHub.Publish(subscribingEvent);
            return subscribingEvent;
        }

        private Message GetSubscriptionSucceededResponse(Message request, IClient client)
        {
            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful =  true,
                           clientId = client.ID,
                           subscription = request.subscription
                       };
        }

        private Message GetSubscriptionFailedResponse(Message request, IClient client, string cancellationReason)
        {
            // The subscription failed response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_44

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = false,
                           clientId = client.ID,
                           subscription = request.subscription,
                           error = string.Format("403:{0},{1}:{2}", client.ID, this.ChannelName, cancellationReason)
                       };
        }
    }
}