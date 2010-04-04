// ReSharper disable InconsistentNaming

using System.Collections.Generic;

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_message_for_forwarding_to_a_channel_with_no_subscribers : ForwardingHandlerScenario
    {
        Because of = () =>
            result = SUT.HandleMessage(request);

        It should_raise_a_publishing_event_with_the_request_message = () =>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_not_return_a_response_message = () =>
            result.Message.ShouldBeNull();

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_message_for_forwarding_to_a_channel_with_two_subscribers : ForwardingHandlerScenario
    {
        Establish context = () =>
        {
            clientsSubscribedToChannel.Add(client1);
            clientsSubscribedToChannel.Add(client2);
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        It should_raise_a_publishing_event_with_the_request_message = () =>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_send_the_forwarding_message_with_correct_properties_to_the_first_client = () =>
            client1.ShouldHaveHadCalled(x => x.Enqueue(Arg<Message>.List.Element(0, Property.AllPropertiesMatch(expectedForwardMessage))));

        It should_send_the_forwarding_message_with_correct_properties_to_the_second_client = () =>
            client2.ShouldHaveHadCalled(x => x.Enqueue(Arg<Message>.List.Element(0, Property.AllPropertiesMatch(expectedForwardMessage))));

        It should_not_return_a_response_message = () =>
            result.Message.ShouldBeNull();
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_forwarding_a_message_to_a_channel_with_2_subscribers_and_the_sender_is_also_subscribed_to_it : ForwardingHandlerScenario
    {
        Establish context = () =>
        {
            clientsSubscribedToChannel.Add(client);
            clientsSubscribedToChannel.Add(client1);
            clientsSubscribedToChannel.Add(client2);
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        It should_raise_a_publishing_event_with_the_request_message = () =>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_send_the_forwarding_message_with_correct_properties_to_the_first_client = () =>
            client1.ShouldHaveHadCalled(x => x.Enqueue(Arg<Message>.List.Element(0, Property.AllPropertiesMatch(expectedForwardMessage))));

        It should_send_the_forwarding_message_with_correct_properties_to_the_second_client = () =>
            client2.ShouldHaveHadCalled(x => x.Enqueue(Arg<Message>.List.Element(0, Property.AllPropertiesMatch(expectedForwardMessage))));

        It should_send_the_forwarding_message_back_to_the_requesting_client = () =>
        {
            result.Message.channel.ShouldEqual(expectedForwardMessage.channel);
            result.Message.data.ShouldEqual(expectedForwardMessage.data);
        };
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_forwarding_a_message_to_a_channel_with_2_subscribers_and_the_publishing_event_is_cancelled : ForwardingHandlerScenario
    {
        private const string CancellationReason = "ThisIsTheCancellationReasonForForwarding";

        Establish context = () =>
        {
            EventHub.Subscribe<PublishingEvent>(x =>
            {
                x.Cancel = true;
                x.CancellationReason = CancellationReason;
            });

            clientsSubscribedToChannel.Add(client1);
            clientsSubscribedToChannel.Add(client2);
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_raise_a_publishing_event_with_the_request_message = () =>
            eventHubMonitor.RaisedEvent<PublishingEvent>().Message.ShouldEqual(request);

        It should_not_send_any_message_to_the_first_client = () =>
            client1.ShouldNotHaveHadCalled(x => x.Enqueue(Arg<Message>.Is.Anything));

        It should_not_send_any_message_to_the_second_client = () =>
            client2.ShouldNotHaveHadCalled(x => x.Enqueue(Arg<Message>.Is.Anything));

        It should_send_an_unsuccessful_response = () =>
            result.Message.successful.ShouldEqual(false);

        It should_send_an_unsuccessful_response_with_error_equal_to_cancellation_reason = () =>
            result.Message.error.ShouldEqual(CancellationReason);
    }

    public abstract class ForwardingHandlerScenario : MessageHandlerScenario<ForwardingHandler>
    {
        protected static IClient client1;
        protected static IClient client2;
        protected static List<IClient> clientsSubscribedToChannel;
        protected static Message expectedForwardMessage;

        Establish context = () =>
        {
            client1 = MockRepository.GenerateStub<IClient>();
            client2 = MockRepository.GenerateStub<IClient>();
            clientsSubscribedToChannel = new List<IClient>();

            Dependency<IClientRepository>().Stub(x => x.WhereSubscribedTo(request.channel)).Return(clientsSubscribedToChannel);

            expectedForwardMessage = new Message
            {
                channel = request.channel,
                data = request.data
                // TODO What other fields should be forwarded?
            };

            eventHubMonitor.StartMonitoring<PublishingEvent>();
        };
    }
}