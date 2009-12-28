using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    /// <summary>
    ///     A message handler which swallows the message without sending to any clients
    /// </summary>
    public class SwallowHandler : IMessageHandler
    {
        public MessageHandlerResult HandleMessage(Message request)
        {
            var e = new PublishingEvent(request);
            EventHub.Publish(e);

            return null;
        }
    }
}