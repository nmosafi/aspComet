using System.Collections.Generic;

namespace AspComet.MessageHandlers
{
    public class MetaConnectHandler : IMessageHandler
    {
        private readonly IClientRepository clientRepository;

        public MetaConnectHandler(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            if (!ClientExistsFor(request))
            {
                return new MessageHandlerResult { Message = GetUnrecognisedClientResponse(request), ShouldWait = false };
            }

            Client client = clientRepository.GetByID(request.clientId);

            bool isFirstConnectRequest = !client.IsConnected;

            client.NotifyConnected();

            return new MessageHandlerResult { Message = GetSuccessfulResponse(request), ShouldWait = !isFirstConnectRequest };
        }

        private bool ClientExistsFor(Message request)
        {
            return request.clientId != null && clientRepository.Exists(request.clientId);
        }

        private static Message GetSuccessfulResponse(Message request)
        {
            // The connect response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and

            return new Message
            {
                id = request.id,
                channel = request.channel,
                successful = true,
                clientId = request.clientId,
                connectionType = "long-polling",
                advice = new Dictionary<string, int>
                { 
                    { "timeout", CometHttpHandler.LongPollDurationInMilliseconds}
                }
            };
        }

        private static Message GetUnrecognisedClientResponse(Message request)
        {
            // The connect failed response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_71

            return new Message
            {
                id = request.id,
                channel = request.channel,
                successful = false,
                clientId = request.clientId,
                connectionType = "long-polling",
                error = "clientId not recognised",
                advice = new Dictionary<string, string>
                { 
                    { "reconnect", "handshake" }
                }
            };
        }

    }
}