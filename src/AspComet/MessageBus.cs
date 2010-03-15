using System;
using System.Collections.Generic;

namespace AspComet
{
    public class MessageBus : IMessageBus
    {
        private readonly IClientRepository clientRepository;
        private readonly Func<IMessagesProcessor> messagesProcessorFactoryMethod;

        public MessageBus(IClientRepository clientRepository, Func<IMessagesProcessor> messagesProcessorFactoryMethod)
        {
            this.clientRepository = clientRepository;
            this.messagesProcessorFactoryMethod = messagesProcessorFactoryMethod;
        }

        public void HandleMessages(Message[] messages, ICometAsyncResult asyncResult)
        {
            // Do this before we process the messages in case it's a disconnect
            IClient sendingClient = GetSenderOf(messages);

            IMessagesProcessor processor = this.CreateProcessorAndProcess(messages);

            if (sendingClient == null)
            {
                asyncResult.CompleteRequestWithMessages(processor.Result);
                return;
            }

            if (sendingClient.CurrentAsyncResult != null)
            {
                sendingClient.FlushQueue();
            }

            sendingClient.CurrentAsyncResult = asyncResult;
            sendingClient.Enqueue(processor.Result);

            if (processor.ShouldSendResultStraightBackToClient)
            {
                sendingClient.FlushQueue();
            }
        }

        private IMessagesProcessor CreateProcessorAndProcess(IEnumerable<Message> messages)
        {
            IMessagesProcessor processor = this.messagesProcessorFactoryMethod();
            processor.Process(messages);
            return processor;
        }

        private IClient GetSenderOf(IEnumerable<Message> messages)
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

            IClient sendingClient = null;
            if (sendingClientId != null)
            {
                sendingClient = this.clientRepository.GetByID(sendingClientId);
            }
            return sendingClient;
        }
    }
}
