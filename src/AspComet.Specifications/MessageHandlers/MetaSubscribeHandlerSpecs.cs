// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_subscribe_message : MessageHandlerScenario
    {
        static IClientRepository clientRepository;
        static IClient client;
        static MetaSubscribeHandler metaSubscribeHandler;
        static SubscribedEvent subscribedEventWhichWasRaised;
        static SubscribingEvent subscribingEventWhichWasRaised;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();

            clientRepository = MockRepository.GenerateStub<IClientRepository>();
            clientRepository.Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

            metaSubscribeHandler = new MetaSubscribeHandler(clientRepository);

            subscribingEventWhichWasRaised = null;
            subscribedEventWhichWasRaised = null;
            EventHub.Subscribe<SubscribingEvent>(ev => 
                subscribingEventWhichWasRaised = ev);
            EventHub.Subscribe<SubscribedEvent>(ev => subscribedEventWhichWasRaised = ev);
        };

        Because of = () =>
            result = metaSubscribeHandler.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_retrieve_the_client_using_the_client_id_in_the_message = () =>
            clientRepository.ShouldHaveHadCalled(x => x.GetByID(request.clientId));

        It should_publish_a_subscribing_event_with_the_client_which_sent_the_message = () =>
            subscribingEventWhichWasRaised.Client.ShouldEqual(client);

        It should_publish_a_subscribing_event_with_the_channel_being_subscribed_to = () =>
            subscribingEventWhichWasRaised.Channel.ShouldEqual(request.subscription);

        It should_subscribe_the_client_to_the_channel = () =>
            client.ShouldHaveHadCalled(x => x.SubscribeTo(request.subscription));

        It should_publish_a_subscribed_event_with_the_client_which_sent_the_message = () =>
            subscribedEventWhichWasRaised.Client.ShouldEqual(client);

        It should_publish_a_subscribed_event_with_the_channel_being_subscribed_to = () =>
            subscribedEventWhichWasRaised.Channel.ShouldEqual(request.subscription);

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_return_a_message_with_the_subscription_of_the_channel_being_subscribed_to = () =>
            result.Message.subscription.ShouldEqual(request.subscription);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }
}