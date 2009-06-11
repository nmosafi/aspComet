using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageBus
    {
        private readonly MessageHandlerCollection metaHandlers = new MessageHandlerCollection();
        private readonly IClientRepository clientRepository;

        public MessageBus(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
            this.InitialiseMetaHandlers();
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
            lock (this.clientRepository)
            {
                // Use a strong RNG to generate a 20 random character base64 client ID.
                // Do lookups in client repository to ensure uniqueness of ID.
                string clientID;
                var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                do
                {
                    byte[] bytes = new byte[15];
                    rng.GetBytes(bytes);
                    clientID = Convert.ToBase64String(bytes);
                }
                while (this.clientRepository.ContainsID(clientID));

                // Create, add and return client.
                Client client = new Client(clientID);
                this.clientRepository.Add(client);
                return client;
            }
        }

        public void HandleMessages(Message[] messages, CometAsyncResult asyncResult)
        {
            Client source = this.GetSourceFrom(messages);
            if (source != null)
            {
                if (source.CurrentAsyncResult == null)
                {
                    source.CurrentAsyncResult = asyncResult;
                }
                else
                {
                    source = null;
                }
            }

            List<Message> response = new List<Message>();
            bool sendResultStraightAway = false;

            foreach (Message message in messages)
            {
                try
                {
                    if (message.channel == null)
                    {
                        throw new Exception("Channel is null");
                    }

                    if (message.channel.StartsWith("/meta/"))
                    {
                        IMessageHandler handler = this.metaHandlers[message.channel];
                        if (handler == null)
                        {
                            throw new Exception("Unknown meta channel " + message.channel);
                        }

                        if (!handler.ShouldWait)
                        {
                            sendResultStraightAway = true;
                        }

                        response.Add(handler.HandleMessage(this, message));
                    }
                    else
                    {
                        // Remove clientId from message before delivering to subscribers.
                        message.clientId = null;
                        foreach (Client client in this.clientRepository.WhereSubscribedTo(message.channel))
                        {
                            client.Enqueue(message);
                            client.FlushQueue();
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.Add(new Message { channel = message.channel, error = exception.Message });
                    sendResultStraightAway = true;
                }
            }

            if (source == null)
            {
                asyncResult.ResponseMessages = response;
                asyncResult.Complete();
            }
            else
            {
                source.Enqueue(response.ToArray());
                if (sendResultStraightAway)
                {
                    source.FlushQueue();
                }
            }
        }

        private Client GetSourceFrom(IEnumerable<Message> messages)
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

        private void InitialiseMetaHandlers()
        {
            this.metaHandlers.Add(new MetaHandshakeHandler());
            this.metaHandlers.Add(new MetaConnectHandler());
            this.metaHandlers.Add(new MetaDisconnectHandler());
            this.metaHandlers.Add(new MetaSubscribeHandler());
            this.metaHandlers.Add(new MetaUnsubscribeHandler());
        }

        private class MessageHandlerCollection : KeyedCollection<string, IMessageHandler>
        {
            protected override string GetKeyForItem(IMessageHandler item)
            {
                return item.ChannelName;
            }
        }
    }
}
