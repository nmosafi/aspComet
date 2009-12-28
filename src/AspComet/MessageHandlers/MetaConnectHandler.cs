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
            // First, check we have a client
            if (request.clientId == null || !clientRepository.Exists(request.clientId))
            {
                return new MessageHandlerResult { Message = GetUnrecognisedClientResponse(request), ShouldWait = false };
            }

            Client client = clientRepository.GetByID(request.clientId);

            client.NotifyConnected();

            return new MessageHandlerResult { Message = GetSuccessfulResponse(request), ShouldWait = client.IsConnected };
        }

        private static Message GetSuccessfulResponse(Message request)
        {
            // The connect response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and

            Dictionary<string, int> connectAdvice = new Dictionary<string, int>();
            connectAdvice["timeout"] = CometHttpHandler.LongPollDuration;
            return new Message
            {
                id = request.id,
                channel = request.channel,
                successful = true,
                clientId = request.clientId,
                connectionType = "long-polling",
                advice = connectAdvice,
            };
        }

        private static Message GetUnrecognisedClientResponse(Message request)
        {
            // The connect failed response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_71

            Dictionary<string, string> connectAdvice = new Dictionary<string, string>();
            connectAdvice["reconnect"] = "handshake";
            return new Message
            {
                id = request.id,
                channel = request.channel,
                successful = false,
                clientId = request.clientId,
                connectionType = "long-polling",
                error = "clientId not recognised",
                advice = connectAdvice,
            };
        }

    }
}