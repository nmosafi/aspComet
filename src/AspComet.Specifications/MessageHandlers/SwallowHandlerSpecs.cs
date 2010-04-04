// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

namespace AspComet.Specifications.MessageHandlers
{
    public class when_handling_a_message_which_should_be_swallowed : MessageHandlerScenario<SwallowHandler>
    {
        Establish context = () =>
            eventHubMonitor.StartMonitoring<PublishingEvent>();

        Because of = () =>
            result = SUT.HandleMessage(request);

        It should_raise_a_published_event_with_the_request_message =()=>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_not_return_a_result = () =>
            result.ShouldBeNull();
    }
}
