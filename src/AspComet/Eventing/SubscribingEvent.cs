namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client is subscribing to a channel
    /// </summary>
    public class SubscribingEvent : CancellableEvent
    {
        private readonly IClient client;
        private readonly string channel;

        /// <summary>
        ///     Initialises a new instance of <see cref="SubscribingEvent"/> class
        /// </summary>
        /// <param name="client">The client which is subscribing</param>
        /// <param name="channel">The channel which is being subscribed to</param>
        public SubscribingEvent(IClient client, string channel)
        {
            this.client = client;
            this.channel = channel;
        }

        /// <summary>
        ///     Gets the client which is subscribing
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }

        /// <summary>
        ///     Gets the channel which is being subscribed to
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }
    }
}
