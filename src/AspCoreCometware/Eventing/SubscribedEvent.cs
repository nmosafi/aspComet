namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client has subscribed to a channel
    /// </summary>
    public class SubscribedEvent : IChannelEvent
    {
        private readonly IClient client;
        private readonly string channel;

        /// <summary>
        ///     Initialises a new instance of the <see cref="SubscribedEvent"/> class
        /// </summary>
        /// <param name="client">The client which has subscribed</param>
        /// <param name="channel">The channel which was subscribed to</param>
        public SubscribedEvent(IClient client, string channel)
        {
            this.client = client;
            this.channel = channel;
        }

        /// <summary>
        ///     Gets the client which has subscribed
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }

        /// <summary>
        ///     Gets the channel which was subscribed to
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }
    }
}
