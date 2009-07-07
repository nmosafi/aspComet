using System;
using System.Collections.Generic;

using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageBus : IMessageBus
    {
        private readonly IClientRepository clientRepository;
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly object clientRepositorySyncRoot = new object();

        public MessageBus(IClientRepository clientRepository, IClientIDGenerator clientIDGenerator)
        {
            this.clientRepository = clientRepository;
            this.clientIDGenerator = clientIDGenerator;
        }

        public Client GetClient(string clientID)
        {
            return this.clientRepository.GetByID(clientID);
        }

        public void RemoveClient(string clientID)
        {
            this.clientRepository.RemoveByID(clientID);
        }

        public Client CreateClient()
        {
            string clientID = this.clientIDGenerator.GenerateClientID();
            Client client = new Client(clientID);

            lock (this.clientRepositorySyncRoot)
            {
                this.clientRepository.Add(client);
            }
            return client;
        }

        public void HandleMessages(Message[] messages, CometAsyncResult asyncResult)
        {
            List<Message> response = new List<Message>();
            bool shouldSendResultStraightBackToClient = false;

            foreach (Message msg in messages)
            {
                IMessageHandler handler = GetMessageHandler(msg.channel);
                response.Add(handler.HandleMessage(this, msg));
                shouldSendResultStraightBackToClient |= !handler.ShouldWait;
            }

            Client sender = this.GetSenderOf(messages);
            if (shouldSendResultStraightBackToClient || sender == null || sender.CurrentAsyncResult != null)
            {
                asyncResult.ResponseMessages = response;
                asyncResult.Complete();
                return;
            }

            sender.CurrentAsyncResult = asyncResult;
            sender.Enqueue(response.ToArray());
        }

        private Client GetSenderOf(IEnumerable<Message> messages)
        {
            Client sendingClient = null;
            foreach (Message message in messages)
            {
                if (message.clientId == null)
                {
                    return null;
                }

                Client client = this.clientRepository.GetByID(message.clientId);
                if (sendingClient != null && sendingClient != client)
                {
                    throw new Exception("All messages must have the same client");
                }
                sendingClient = client;
            }

            return sendingClient;
        }

        private IMessageHandler GetMessageHandler(string channelName)
        {
            // If no channel name is given, no handler can be found.
            if (channelName == null) new ExceptionHandler("Empty channel field in request.");

            // Use switch statement to identify known meta channels.
            if (channelName.StartsWith("/meta/"))
            {
                switch (channelName.Substring(6))
                {
                    case "connect": return new MetaConnectHandler();
                    case "disconnect": return new MetaDisconnectHandler();
                    case "handshake": return new MetaHandshakeHandler();
                    case "subscribe": return new MetaSubscribeHandler();
                    case "unsubscribe": return new MetaUnsubscribeHandler();
                    default: return new ExceptionHandler("Unknown meta channel.");
                }
            }
            
            if (channelName.StartsWith("/service/"))
            {
                // TODO Return some class defined by application, or some kind of "null handler"?
            }

            // If neither meta nor service, pass-thru as ordinary publish.
            return new PassThruHandler(channelName, clientRepository.WhereSubscribedTo(channelName));
        }
    }
}
