using System;
using System.Web;

using AspComet.Eventing;

using Autofac;

using Microsoft.Practices.ServiceLocation;

namespace AspComet.Samples.Chat
{
    public class Global : HttpApplication
    {
        private static IContainer container;

        protected void Application_Start(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------------------------
            //   There are two ways to initialise AspComet.  The simple way to do this is:
            // Setup.AspComet.InANonExtensibleAndNonConfigurableManner();
            //
            //   However, in any application of any size, you will want to configure AspComet using the common
            //   service locator backed onto your application's IoC container.
            // Configuration.InitialiseHttpHandler.WithMessageBus(messageBus);
            //
            //-------------------------------------------------------------------------------------------------

            SetupIoCContainer();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));

            EventHub.Subscribe<HandshakingEvent>(container.Resolve<HandshakeAuthenticator>().CheckHandshake);
            EventHub.Subscribe<PublishingEvent>(container.Resolve<BadLanguageBlocker>().CheckMessage);
            EventHub.Subscribe<SubscribingEvent>(container.Resolve<SubscriptionChecker>().CheckSubscription);
            EventHub.Subscribe<PublishingEvent>("/service/whisper", container.Resolve<Whisperer>().SendWhisper);
            CometHttpHandler.AllowOrigin = "*";
        }

        protected void Application_End(object sender, EventArgs e)
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        private static void SetupIoCContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Let AspComet put its registrations into the container
            foreach (ServiceMetadata metadata in ServiceMetadata.GetMinimumSet())
            {
                if (metadata.IsPerRequest)
                    builder.RegisterType(metadata.ActualType).As(metadata.ServiceType);
                else
                    builder.RegisterType(metadata.ActualType).As(metadata.ServiceType).SingleInstance();
            }

            // Add our own stuff to the container
            builder.RegisterType<AuthenticatedClientFactory>().As<IClientFactory>().SingleInstance();
            builder.RegisterType<HandshakeAuthenticator>().SingleInstance();
            builder.RegisterType<BadLanguageBlocker>().SingleInstance();
            builder.RegisterType<SubscriptionChecker>().SingleInstance();
            builder.RegisterType<Whisperer>().SingleInstance();

            // Set up the common service locator
            container = builder.Build();
        }
    }
}