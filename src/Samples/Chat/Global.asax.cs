using System;
using System.Web;

using AspComet.Eventing;

namespace AspComet.Samples.Chat
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------------------------
            //    There are two ways to initialise AspComet.  You may wish to configure the message bus and its
            //    dependencies yourself, for example you might want to provide your own client ID generator. If
            //    so then use this method:
            // Configuration.InitialiseHttpHandler.WithMessageBus(messageBus);
            //
            //    Otherwise, this is useful - it just gives a new message bus with the default configuration
            // Configuration.InitialiseHttpHandler.WithTheDefaultConfiguration();
            //-------------------------------------------------------------------------------------------------

            // Initialise everything ourselves as we need the client repository for our event handlers
            IClientRepository clientRepository = new InMemoryClientRepository();
            IClientIDGenerator clientIDGenerator = new RngUniqueClientIDGenerator(clientRepository);
            IClientFactory clientFactory = new ClientFactory();
            Configuration.InitialiseHttpHandler.WithMessageBus(new MessageBus(clientRepository, clientIDGenerator, clientFactory));

            // Now create our bad language blocker
            BadLanguageBlocker badLanguageBlocker = new BadLanguageBlocker(clientRepository);
            EventHub.Subscribe<PublishingEvent>(badLanguageBlocker.CheckMessage);

            // And our subscription checker
            SubscriptionChecker subscriptionChecker = new SubscriptionChecker(clientRepository);
            EventHub.Subscribe<SubscribingEvent>(subscriptionChecker.CheckSubscription);
        }
    }
}