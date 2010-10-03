// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

namespace AspComet.Specifications.MessageHandlers
{
    public class when_swallowing_a_message : MessageHandlerScenario<SwallowHandler>
    {
        Establish context = () =>
            eventHubMonitor.StartMonitoring<PublishingEvent>();

        Because of = () =>
            result = SUT.HandleMessage(request);

        It should_raise_a_published_event_with_the_request_message =()=>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_not_return_a_response_message = () =>
            result.Message.ShouldBeNull();

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    public class when_swallowing_a_message_and_the_publishing_event_is_cancelled : MessageHandlerScenario<SwallowHandler>
    {
        private const string CancellationReason = "ThisIsTheCancellationReasonForSwallowing";

        Establish context = () => {
            EventHub.Subscribe<PublishingEvent>(x =>
            {
                x.Cancel = true;
                x.CancellationReason = CancellationReason;
            });
            eventHubMonitor.StartMonitoring<PublishingEvent>();
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_raise_a_published_event_with_the_request_message =()=>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_send_an_unsuccessful_response = () =>
            result.Message.successful.ShouldEqual( false );

        It should_send_an_unsuccessful_response_with_error_equal_to_cancellation_reason = () =>
            result.Message.error.ShouldEqual( CancellationReason );

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

}
