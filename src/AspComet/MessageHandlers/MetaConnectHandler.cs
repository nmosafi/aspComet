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
            IClient client = clientRepository.GetByID(request.clientId);

            if (client == null)
            {
                return new MessageHandlerResult { Message = GetUnrecognisedClientResponse(request), CanTreatAsLongPoll = false };
            }

            bool canTreatAsLongPoll = false;
            if ( client.IsConnected ) {
                canTreatAsLongPoll = client.PendingMessageCount == 0;
            }

            client.NotifyConnected();

            return new MessageHandlerResult { Message = GetSuccessfulResponse(request), CanTreatAsLongPoll = canTreatAsLongPoll };
        }

        private static Message GetSuccessfulResponse(Message request)
        {
            // The connect response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and

            Message response = new Message
            {
                id = request.id,
                channel = request.channel,
                successful = true,
                clientId = request.clientId,
                connectionType = "long-polling",
            };

            response.SetAdvice("timeout", CometHttpHandler.LongPollDurationInMilliseconds);

            return response;
        }

        private static Message GetUnrecognisedClientResponse(Message request)
        {
            // The connect failed response is documented at
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_53 and
            // http://svn.cometd.com/trunk/bayeux/bayeux.html#toc_71

            Message response = new Message
            {
                id = request.id,
                channel = request.channel,
                successful = false,
                clientId = request.clientId,
                connectionType = "long-polling",
                error = "clientId not recognised",
            };

            response.SetAdvice("reconnect", "handshake");

            return response;
        }

    }
}