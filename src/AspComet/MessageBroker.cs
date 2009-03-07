using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageBroker
    {
        private readonly MessageHandlerCollection metaHandlers = new MessageHandlerCollection();
        private readonly IClientRepository clientRepository;

        public MessageBroker(IClientRepository clientRepository)
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
            Client client = new Client();
            this.clientRepository.Add(client);
            return client;
        }

        public void HandleMessages(Message[] messages, CometAsyncResult asyncResult)
        {
            Client source = this.GetSourceFrom(messages);
            if (source != null)
                source.CurrentAsyncResult = asyncResult;

            List<Message> response = new List<Message>();
            bool sendResultStraightAway = false;

            foreach (Message message in messages)
            {
                try
                {
                    if (message.Channel == null)
                    {
                        throw new Exception("Channel is null");
                    }

                    if (message.Channel.StartsWith("/meta/"))
                    {
                        IMessageHandler handler = this.metaHandlers[message.Channel];
                        if (handler == null)
                        {
                            throw new Exception("Unknown meta channel " + message.Channel);
                        }

                        if (!handler.ShouldWait)
                        {
                            sendResultStraightAway = true;
                        }

                        response.Add(handler.HandleMessage(this, message));
                    }
                    else
                    {
                        foreach (Client client in this.clientRepository.WhereSubscribedTo(message.Channel))
                        {
                            client.Enqueue(message);
                            client.FlushQueue();
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.Add(new Message { Channel = message.Channel, Error = exception.Message });
                    sendResultStraightAway = true;
                }
            }

            if (source == null)
            {
                asyncResult.ResponseMessages = response.ToArray();
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
                if (message.ClientID == null)
                {
                    return null;
                }

                Client client = this.clientRepository.GetByID(message.ClientID);
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
