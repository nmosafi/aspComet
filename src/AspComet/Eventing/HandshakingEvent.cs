namespace AspComet.Eventing
{
    /// <summary>
    ///     Raised when a client is attempting to handshake
    /// </summary>
    public class HandshakingEvent : CancellableEvent
    {
        private readonly IClient client;
        private readonly Message handshake;

        /// <summary>
        ///     Initialises a new instance of <see cref="HandshakingEvent"/> class
        /// </summary>
        /// <param name="client">The client which is handshaking</param>
        /// <param name="handshake">The handshake message</param>
        public HandshakingEvent(IClient client, Message handshake)
        {
            this.client = client;
            this.handshake = handshake;
        }

        /// <summary>
        ///     Flag indicating if the client should reconnect or not. Defaults to false if the handshake is cancelled.
        /// </summary>
        public bool Retry { get; set; }

        /// <summary>
        ///     Gets the client which is handshaking
        /// </summary>
        public IClient Client
        {
            get { return this.client; }
        }

        /// <summary>
        ///     Gets the handshake message
        /// </summary>
        public Message Handshake
        {
            get { return this.handshake; }
        }

    }
}
