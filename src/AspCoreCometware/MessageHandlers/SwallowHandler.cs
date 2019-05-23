using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    /// <summary>
    ///     A message handler which swallows the message without forwarding to any clients
    ///     NOTE: I am considering calling this NonForwardingHandler - or maybe it should be combined with ForwardingHandler?!
    /// </summary>
    public class SwallowHandler : IMessageHandler
    {
        public MessageHandlerResult HandleMessage(Message request)
        {
            var e = new PublishingEvent(request);
            EventHub.Publish(e);

            Message msg = null;
            if (e.Cancel)
            {
                msg = new Message
                {
                    id = request.id,
                    clientId = request.clientId,
                    channel = request.channel,
                    successful = false,
                    error = e.CancellationReason
                };
            }

            return new MessageHandlerResult
            {
                Message = msg,
                CanTreatAsLongPoll = false
            };
        }
    }
}