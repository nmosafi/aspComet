namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client disconnects to the message bus
    /// </summary>
    public class DisconnectedEvent : IEvent
    {
        private readonly IClient client;

        /// <summary>
        ///     Initialises a new instance of the <see cref="DisconnectedEvent"/> class
        /// </summary>
        /// <param name="client"></param>
        public DisconnectedEvent(IClient client)
        {
            this.client = client;
        }

        /// <summary>
        ///     Gets the client which disconnected
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }
    }
}
