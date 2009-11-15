namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client is publishing a message to the comet message bus
    /// </summary>
    public class PublishingEvent : ICancellableEvent
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

        /// <summary>
        ///     Gets or sets whether to cancel the event
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets or sets sets the reason for cancellation
        /// </summary>
        public string CancellationReason { get; set; }
    }
}
