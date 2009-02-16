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

        public void InitialiseMetaHandlers()
        {
            this.metaHandlers.Add(new MetaHandshakeHandler());
            this.metaHandlers.Add(new MetaConnectHandler());
            this.metaHandlers.Add(new MetaDisconnectHandler());
            this.metaHandlers.Add(new MetaSubscribeHandler());
            this.metaHandlers.Add(new MetaUnsubscribeHandler());
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

        public Message[] HandleMessage(Message message)
        {
            if (message.Channel == null)
            {
                message.Successful = false;
                message.Error = "No channel";
                return new[] { message };
            }

            try
            {
                List<Message> response = new List<Message>();
                if (message.Channel.StartsWith("/meta/"))
                {
                    IMessageHandler handler = this.metaHandlers[message.Channel];
                    if (handler == null)
                    {
                        throw new Exception("Unknown meta channel " + message.Channel);
                    }
                    response.Add(handler.HandleMessage(this, message));
                    if (handler is MetaHandshakeHandler)
                    {
                        return response.ToArray();
                    }
                }

                Client sendingClient = this.clientRepository.GetByID(message.ClientID);
                if (sendingClient == null)
                {
                    throw new Exception("Sending client cannot be null");
                }

                foreach (Client client in this.clientRepository.WhereSubscribedTo(message.Channel))
                {
                    client.Enqueue(message);
                }

                response.AddRange(sendingClient.WaitForQueuedMessages());
                return response.ToArray();
            }
            catch (Exception exception)
            {
                return new[]
                           {
                               new Message {Channel = message.Channel, Error = exception.Message}
                           };
            }

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
