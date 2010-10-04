namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client is publishing a message to the comet message bus
    /// </summary>
    public class PublishingEvent : CancellableEvent, IChannelEvent
    {
        private readonly Message message;

        /// <summary>
        ///     Initialises a new instance of the <see cref="PublishingEvent"/> class
        /// </summary>
        /// <param name="message"></param>
        public PublishingEvent(Message message)
        {
            this.message = message;
        }

        /// <summary>
        ///     Gets the message being published
        /// </summary>
        public Message Message
        {
            get { return this.message; }
        }

        public string Channel
        {
            get { return this.Message.channel; }
        }
    }
}
