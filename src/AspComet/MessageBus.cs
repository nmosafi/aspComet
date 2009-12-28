using System;
using System.Collections.Generic;

using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageBus : IMessageBus
    {
        private readonly IClientRepository clientRepository;
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly IClientFactory clientFactory;

        public MessageBus(IClientRepository clientRepository, IClientIDGenerator clientIDGenerator, IClientFactory clientFactory)
        {
            this.clientRepository = clientRepository;
            this.clientIDGenerator = clientIDGenerator;
            this.clientFactory = clientFactory;
        }

        public void HandleMessages(Message[] messages, CometAsyncResult asyncResult)
        {
            List<Message> response = new List<Message>();
            bool shouldSendResultStraightBackToClient = false;

            // Get the client who sent this before we process
            // the messages - in case it's a disconnect
            Client sendingClient = this.GetSenderOf(messages);
            foreach (Message msg in messages)
            {
                IMessageHandler handler = GetMessageHandler(msg.channel);
                MessageHandlerResult handlerResult = handler.HandleMessage(msg);
                response.Add(handlerResult.Message);
                shouldSendResultStraightBackToClient |= !handlerResult.ShouldWait;
            }

            if (sendingClient == null)
            {
                asyncResult.ResponseMessages = response;
                asyncResult.Complete();
                return;
            }

            if (sendingClient.CurrentAsyncResult != null)
            {
                sendingClient.FlushQueue();
            }

            sendingClient.CurrentAsyncResult = asyncResult;
            sendingClient.Enqueue(response.ToArray());

            if (shouldSendResultStraightBackToClient)
            {
                sendingClient.FlushQueue();
            }
        }

        private Client GetSenderOf(IEnumerable<Message> messages)
        {
            string sendingClientId = null;
            foreach (Message message in messages)
            {
                if( sendingClientId != null 
                    && message.clientId != null 
                    && sendingClientId != message.clientId)
                {
                    throw new Exception("All messages must have the same client");
                }
                if (message.clientId != null)
                {
                    sendingClientId = message.clientId;
                }
            }

            Client sendingClient = null;
            if (sendingClientId != null && this.clientRepository.Exists(sendingClientId))
            {
                sendingClient = this.clientRepository.GetByID(sendingClientId);
            }
            return sendingClient;
        }

        private IMessageHandler GetMessageHandler(string channelName)
        {
            // If no channel name is given, no handler can be found.
            if (channelName == null) 
                return new ExceptionHandler("Empty channel field in request.");

            // Use switch statement to identify known meta channels.
            if (channelName.StartsWith("/meta/"))
            {
                switch (channelName.Substring(6))
                {
                    case "connect": return new MetaConnectHandler(clientRepository);
                    case "disconnect": return new MetaDisconnectHandler(clientRepository);
                    case "handshake": return new MetaHandshakeHandler(clientIDGenerator, clientFactory, clientRepository);
                    case "subscribe": return new MetaSubscribeHandler(clientRepository);
                    case "unsubscribe": return new MetaUnsubscribeHandler(clientRepository);
                    default: return new ExceptionHandler("Unknown meta channel.");
                }
            }
            
            if (channelName.StartsWith("/service/"))
            {
                return new SwallowHandler(channelName);
            }

            // If neither meta nor service, pass-thru as ordinary publish.
            return new PassThruHandler(channelName, clientRepository.WhereSubscribedTo(channelName));
        }
    }
}
