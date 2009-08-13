namespace AspComet.Eventing
{
    public class SubscribedEvent : IEvent
    {
        public bool Cancel { get; set; }
        private readonly IClient client;
        private readonly string channel;

        public SubscribedEvent(IClient client, string channel)
        {
            this.client = client;
            this.channel = channel;
        }

        public IClient Client
        {
            get { return this.client; }
        }

        public string Channel
        {
            get { return this.channel; }
        }
    }
}
