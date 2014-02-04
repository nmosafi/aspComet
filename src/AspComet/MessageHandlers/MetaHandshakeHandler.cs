using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaHandshakeHandler : IMessageHandler
    {
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly IClientFactory clientFactory;
        private readonly IClientWorkflowManager clientWorkflowManager;

        public MetaHandshakeHandler(IClientIDGenerator clientIDGenerator, IClientFactory clientFactory, IClientWorkflowManager clientWorkflowManager)
        {
            this.clientIDGenerator = clientIDGenerator;
            this.clientFactory = clientFactory;
            this.clientWorkflowManager = clientWorkflowManager;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            IClient client = CreateClient();

            HandshakingEvent handshakingEvent = new HandshakingEvent(client, request);
            EventHub.Publish(handshakingEvent);

            if (handshakingEvent.Cancel) 
            {
                return new MessageHandlerResult
                {
                    Message = GetFailedHandshakeResponse(request, handshakingEvent.CancellationReason, handshakingEvent.Retry),
                    CanTreatAsLongPoll = false
                };
            }

            this.clientWorkflowManager.RegisterClient(client);

            HandshakenEvent handshakenEvent = new HandshakenEvent(client);
            EventHub.Publish(handshakenEvent);

            return new MessageHandlerResult { Message = GetSuccessfulResponse(request, client), CanTreatAsLongPoll = false };
        }

        private IClient CreateClient()
        {
            string clientID = clientIDGenerator.GenerateClientID();
            IClient client = clientFactory.CreateClient(clientID);
            return client;
        }

        private static Message GetSuccessfulResponse(Message request, IClient client)
        {
            // The handshaks success response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            Message message = new Message
            {
                channel = request.channel,
                version = "1.0",
                supportedConnectionTypes = new[] { "long-polling", "callback-polling" },
                clientId = client.ID,
                successful = true,
                id = request.id,
            };

            message.SetAdvice("reconnect", "retry");

            return message;
        }

        private static Message GetFailedHandshakeResponse(Message request, string cancellationReason, bool retry)
        {
            // The handshake failed response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            Message message = new Message
            {
                channel = request.channel,
                error = cancellationReason,
                supportedConnectionTypes = new[] { "long-polling", "callback-polling" },
                version = "1.0",
                successful = false,
                id = request.id,
            };

            if (!retry)
            {
                message.SetAdvice("reconnect", "none");
            }

            return message;
        }

    }
}