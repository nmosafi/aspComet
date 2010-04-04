// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_subscribe_message : MetaSubscribeHandlerScenario
    {
        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        Behaves_like<ItHasHandledASubscribeMessage> has_handled_a_subscribe_message;

        It should_subscribe_the_client_to_the_channel = () =>
            client.ShouldHaveHadCalled(x => x.SubscribeTo(request.subscription));

        It should_publish_a_subscribed_event_with_the_client_which_sent_the_message = () =>
            eventHubMonitor.RaisedEvent<SubscribedEvent>().Client.ShouldEqual(client);

        It should_publish_a_subscribed_event_with_the_channel_being_subscribed_to = () =>
            eventHubMonitor.RaisedEvent<SubscribedEvent>().Channel.ShouldEqual(request.subscription);

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_return_a_message_with_the_subscription_of_the_channel_being_subscribed_to = () =>
            result.Message.subscription.ShouldEqual(request.subscription);
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_subscribe_message_which_was_cancelled : MetaSubscribeHandlerScenario
    {
        private const string CancellationReason = "ThisIsCancelled!";

        Establish context = () =>
            EventHub.Subscribe<SubscribingEvent>(ev => { ev.Cancel = true; ev.CancellationReason = CancellationReason; });

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        Behaves_like<ItHasHandledASubscribeMessage> has_handled_a_subscribe_message;

        It should_not_publish_a_subscribed_event = () =>
            eventHubMonitor.RaisedEvent<SubscribedEvent>().ShouldBeNull();

        It should_not_subscribe_the_client_to_the_channel = () =>
            client.ShouldNotHaveHadCalled(x => x.SubscribeTo(request.subscription));

        It should_return_an_unsuccessful_message = () =>
            result.Message.successful.ShouldEqual(false);

        It should_return_a_message_with_403_error_containing_the_client_and_the_channel_and_the_cancellation_reason_of_the_subscribing_event = () =>
            result.Message.error.ShouldEqual(string.Format("403:{0},{1}:{2}", request.clientId, request.channel, CancellationReason));
    }

    [Behaviors]
    public class ItHasHandledASubscribeMessage : MetaSubscribeHandlerScenario
    {
        It should_retrieve_the_client_using_the_client_id_in_the_message = () =>
            Dependency<IClientRepository>().ShouldHaveHadCalled(x => x.GetByID(request.clientId));

        It should_publish_a_subscribing_event_with_the_client_which_sent_the_message = () =>
            eventHubMonitor.RaisedEvent<SubscribingEvent>().Client.ShouldEqual(client);

        It should_publish_a_subscribing_event_with_the_channel_being_subscribed_to = () =>
            eventHubMonitor.RaisedEvent<SubscribingEvent>().Channel.ShouldEqual(request.subscription);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    public abstract class MetaSubscribeHandlerScenario : MessageHandlerScenario<MetaSubscribeHandler>
    {
        Establish context = () =>
        {
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

            eventHubMonitor.StartMonitoring<SubscribingEvent, SubscribedEvent>();
        };
    }
}