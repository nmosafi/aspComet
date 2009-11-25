namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client connects to the message bus
    /// </summary>
    public class HandshakenEvent : IEvent
    {
        private readonly IClient client;

        /// <summary>
        ///     Initialises a new instance of the <see cref="HandshakenEvent"/> class
        /// </summary>
        /// <param name="client"></param>
        public HandshakenEvent(IClient client)
        {
            this.client = client;
        }

        /// <summary>
        ///     Gets the client which connected
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }
    }
}
