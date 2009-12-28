namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client unsubscribes from a channel
    /// </summary>
    class UnsubscribedEvent : IEvent
    {
        private readonly IClient client;
        private readonly string channel;

        /// <summary>
        ///     Initialises a new instance of the <see cref="UnsubscribedEvent"/> class
        /// </summary>
        public UnsubscribedEvent(IClient client, string channel)
        {
            this.client = client;
            this.channel = channel;
        }

        /// <summary>
        ///     Gets the client which has unsubscribed
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }

        /// <summary>
        ///     Gets the channel which was unsubscribed from
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }
    }
}
