using AspComet.Eventing;
using AspComet;
namespace AspCometCoreApp
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
                ev.CancellationReason = "SubscriptionChecker says: You've been arbitrarily stopped from joining this channel. Just because you're clientID contains the letter 'A'. Isn't that unfair?";
            }

        }

    }
}