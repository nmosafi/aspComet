using System.Collections.Generic;

using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaHandshakeHandler : IMessageHandler
    {
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly IClientFactory clientFactory;
        private readonly IClientRepository clientRepository;

        public MetaHandshakeHandler(IClientIDGenerator clientIDGenerator, IClientFactory clientFactory, IClientRepository clientRepository)
        {
            this.clientIDGenerator = clientIDGenerator;
            this.clientFactory = clientFactory;
            this.clientRepository = clientRepository;
        }

        public string ChannelName
        {
            get { return "/meta/handshake"; }
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            Client client = CreateClient();

            var handshakingEvent = new HandshakingEvent(client, request);
            EventHub.Publish(handshakingEvent);
            if (handshakingEvent.Cancel) 
            {
                clientRepository.RemoveByID(client.ID);
                return new MessageHandlerResult
                {
                    Message = GetFailedHandshakeResponse(request, handshakingEvent.CancellationReason, handshakingEvent.Retry),
                    ShouldWait = false
                };
            }

            var handshakenEvent = new HandshakenEvent(client);
            EventHub.Publish(handshakenEvent);

            return new MessageHandlerResult { Message = GetSuccessfulResponse(request, client), ShouldWait = false };
        }

        private Client CreateClient()
        {
            string clientID = clientIDGenerator.GenerateClientID();
            Client client = clientFactory.CreateClient(clientID);
            this.clientRepository.Add(client);
            return client;
        }

        private Message GetSuccessfulResponse(Message request, IClient client)
        {
            // The handshaks success response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            return new Message
            {
                channel = this.ChannelName,
                version = "1.0",
                supportedConnectionTypes = new[] { "long-polling" },
                clientId = client.ID,
                successful = true,
                id = request.id,
                advice = new Dictionary<string, string>
                {
                    { "reconnect", "retry" }
                },
            };
        }

        private Message GetFailedHandshakeResponse(Message request, string cancellationReason, bool retry)
        {
            // The handshake failed response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            Dictionary<string, string> handshakeAdvice = null;
            if (!retry)
            {
                handshakeAdvice = new Dictionary<string,string>();
                handshakeAdvice["reconnect"] = "none";
            }

            return new Message
            {
                channel = this.ChannelName,
                successful = false,
                error = cancellationReason,
                supportedConnectionTypes = new[] { "long-polling" },
                version = "1.0",
                id = request.id,
                advice = handshakeAdvice,
            };
        }

    }
}