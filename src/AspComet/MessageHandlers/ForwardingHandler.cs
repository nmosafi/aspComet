using System.Collections.Generic;
using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    /// <summary>
    ///     A message handler which fowards messages through to any clients subscribed to the
    ///     channel of the request message 
    /// </summary>
    public class ForwardingHandler : IMessageHandler
    {
        private readonly IClientRepository clientRepository;

        public ForwardingHandler(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            PublishingEvent e = this.PublishPublishingEvent(request);

            Message messageToSendToSender = null;

            if (e.Cancel)
            {
                messageToSendToSender = GetForwardingFailedResponse(request, e.CancellationReason);
            }

            Message forwardMessage = GetForwardMessage(request);

            bool shouldFowardToSender = SendMessageToRecipients(request, forwardMessage);

            if (shouldFowardToSender)
            {
                messageToSendToSender = forwardMessage;
            }

            return new MessageHandlerResult { Message = messageToSendToSender, CanTreatAsLongPoll = false };
        }

        private bool SendMessageToRecipients(Message request, Message forwardMessage)
        {
            IEnumerable<IClient> recipients = clientRepository.WhereSubscribedTo(request.channel);

            bool shouldFowardToSender = false;
            foreach (Client client in recipients)
            {
                if (client.ID == request.clientId)
                {
                    shouldFowardToSender = true;
                }
                else
                {
                    client.Enqueue(forwardMessage);
                    client.FlushQueue();
                }
            }
            return shouldFowardToSender;
        }

        private static Message GetForwardMessage(Message request)
        {
            return new Message
            {
                channel = request.channel,
                data = request.data
                // TODO What other fields should be forwarded?
            };
        }

        private static Message GetForwardingFailedResponse(Message request, string cancellationReason)
        {
            return new Message
            {
                successful = false,
                channel = request.channel,
                error = cancellationReason
            };
        }

        private PublishingEvent PublishPublishingEvent(Message request)
        {
            PublishingEvent e = new PublishingEvent(request);
            EventHub.Publish(e);
            return e;
        }
    }
}
