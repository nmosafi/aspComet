using AspComet.Eventing;

namespace AspComet.Samples.Chat
{
    public class SubscriptionChecker
    {
        private IClientRepository clientRepository;

        public SubscriptionChecker(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public void CheckSubscription(SubscribingEvent ev)
        {
            if (ev.Client.ID.Contains("A"))
            {
                ev.Cancel = true;
                ev.CancellationReason = "You've been arbitrarily stopped from joining this channel";
            }

        }

    }
}