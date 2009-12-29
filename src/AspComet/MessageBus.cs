using System;
using System.Collections.Generic;
using System.Linq;

using AspComet.MessageHandlers;

namespace AspComet
{
    public class MessageBus : IMessageBus
    {
        private readonly IClientRepository clientRepository;
        private readonly IClientIDGenerator clientIDGenerator;
        private readonly IClientFactory clientFactory;
        private readonly IMessageHandler metaConnectHandler;
        private readonly IMessageHandler metaDisconnectHandler;
        private readonly IMessageHandler metaHandshakeHandler;
        private readonly IMessageHandler metaSubscribeHandler;
        private readonly IMessageHandler metaUnsubscribeHandler;
        private readonly IMessageHandler swallowHandler;

        public MessageBus(IClientRepository clientRepository, IClientIDGenerator clientIDGenerator, IClientFactory clientFactory)
        {
            this.clientRepository = clientRepository;
            this.clientIDGenerator = clientIDGenerator;
            this.clientFactory = clientFactory;

            this.metaConnectHandler = new MetaConnectHandler(this.clientRepository);
            this.metaDisconnectHandler = new MetaDisconnectHandler(this.clientRepository);
            this.metaHandshakeHandler = new MetaHandshakeHandler(this.clientIDGenerator, this.clientFactory, this.clientRepository);
            this.metaSubscribeHandler = new MetaSubscribeHandler(this.clientRepository);
            this.metaUnsubscribeHandler = new MetaUnsubscribeHandler(this.clientRepository);
            this.swallowHandler = new SwallowHandler();
        }

        public void HandleMessages(Message[] messages, CometAsyncResult asyncResult)
        {
            // Do this before we process the messages in case it's a disconnect
            Client sendingClient = GetSenderOf(messages);

            MessagesProcessor processor = new MessagesProcessor(this);
            processor.Process(messages);

            if (sendingClient == null)
            {
                asyncResult.CompleteRequestWithMessages(processor.Response);
                return;
            }

            if (sendingClient.CurrentAsyncResult != null)
            {
                sendingClient.FlushQueue();
            }

            sendingClient.CurrentAsyncResult = asyncResult;
            sendingClient.Enqueue(processor.Response);

            if (processor.ShouldSendResultStraightBackToClient)
            {
                sendingClient.FlushQueue();
            }
        }

        private Client GetSenderOf(IEnumerable<Message> messages)
        {
            string sendingClientId = null;
            foreach (Message message in messages)
            {
                if (sendingClientId != null
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
            if (channelName == null)
            {
                return new ExceptionHandler("Empty channel field in request.");
            }

            if (channelName.StartsWith("/meta/"))
            {
                return GetMetaHandler(channelName);
            }

            if (channelName.StartsWith("/service/"))
            {
                return swallowHandler;
            }

            return new PassThruHandler(channelName, clientRepository.WhereSubscribedTo(channelName));
        }

        private IMessageHandler GetMetaHandler(string channelName)
        {
            switch (channelName.Substring(6))
            {
                case "connect": return metaConnectHandler;
                case "disconnect": return metaDisconnectHandler;
                case "handshake": return metaHandshakeHandler;
                case "subscribe": return metaSubscribeHandler;
                case "unsubscribe": return metaUnsubscribeHandler;
                default: return new ExceptionHandler("Unknown meta channel.");
            }
        }

        private class MessagesProcessor
        {
            private readonly MessageBus messageBus;
            private readonly List<Message> response = new List<Message>();

            public MessagesProcessor(MessageBus messageBus)
            {
                this.messageBus = messageBus;
            }

            public bool ShouldSendResultStraightBackToClient { get; private set; }
            public IEnumerable<Message> Response { get { return response; } }

            public void Process(IEnumerable<Message> messages)
            {
                foreach (Message message in messages)
                {
                    this.Process(message);
                }
            }

            private void Process(Message message)
            {
                IMessageHandler handler = this.messageBus.GetMessageHandler(message.channel);
                MessageHandlerResult handlerResult = handler.HandleMessage(message);
                this.response.Add(handlerResult.Message);
                this.ShouldSendResultStraightBackToClient |= !handlerResult.ShouldWait;
            }
        }
    }
}
