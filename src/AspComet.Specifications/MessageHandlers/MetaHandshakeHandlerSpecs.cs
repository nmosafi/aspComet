// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_handshake_message : MetaHandshakeMessageHandlerScenario
    {
        Because of =()=>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMetaHandshakeMessage> has_handled_a_meta_handshake_message;

        It should_register_the_client_with_the_client_workflow_manager = () =>
            Dependency<IClientWorkflowManager>().ShouldHaveHadCalled(x => x.RegisterClient(client));

        It should_publish_a_handshaken_event_with_the_client_which_was_created = () =>
            eventHubMonitor.RaisedEvent<HandshakenEvent>().Client.ShouldEqual(client);

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_return_a_message_with_a_client_id_equal_to_the_generated_one = () =>
            result.Message.clientId.ShouldEqual(GeneratedClientID);

        It should_return_a_message_with_reconnect_advice_of_retry = ()=>
            result.Message.GetAdvice<string>("reconnect").ShouldEqual("retry");
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_handshake_message_which_has_been_cancelled : MetaHandshakeMessageHandlerScenario
    {
        const string CancellationReason = "This is the cancellation reason";

        Establish context = () =>
            EventHub.Subscribe<HandshakingEvent>(e =>
            {
                e.Cancel = true;
                e.CancellationReason = CancellationReason;
            });

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMetaHandshakeMessage> has_handled_a_meta_handshake_message;

        It should_not_register_the_client_with_the_client_workflow_manager = () =>
            Dependency<IClientWorkflowManager>().ShouldNotHaveHadCalled(x => x.RegisterClient(Arg<IClient>.Is.Anything));

        It should_not_publish_a_handshaken_event = () =>
            eventHubMonitor.RaisedEvent<HandshakenEvent>().ShouldBeNull();

        It should_return_an_unsuccessul_message = () =>
            result.Message.successful.ShouldEqual(false);

        It should_return_a_message_with_reconnect_advice_of_retry = () =>
            result.Message.GetAdvice<string>("reconnect").ShouldEqual("none");

        It should_return_a_message_with_an_error_message_equal_to_the_cancellation_reason_of_the_event = () =>
            result.Message.error.ShouldEqual(CancellationReason);
    }

    [Behaviors]
    public class ItHasHandledAMetaHandshakeMessage : MetaHandshakeMessageHandlerScenario
    {
        It should_create_a_client_with_the_id_generated_by_the_client_id_generator = () =>
            Dependency<IClientFactory>().ShouldHaveHadCalled(x => x.CreateClient(GeneratedClientID));

        It should_publish_a_handshaking_event_with_the_client_which_was_created = () =>
            eventHubMonitor.RaisedEvent<HandshakingEvent>().Client.ShouldEqual(client);

        It should_publish_a_handshaking_event_with_the_request_message = () =>
            eventHubMonitor.RaisedEvent<HandshakingEvent>().Handshake.ShouldEqual(request);

        It should_return_a_message_with_the_same_id_as_the_request_message = () =>
            result.Message.id.ShouldEqual(request.id);

        It should_return_a_message_with_version_1_0 = () =>
            result.Message.version.ShouldEqual("1.0");

        It should_return_a_message_with_the_same_channel_as_the_request_message = () =>
            result.Message.channel.ShouldEqual(request.channel);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();

        It should_return_a_message_with_supported_connection_types_of_long_polling = () =>
            result.Message.supportedConnectionTypes.ShouldContainOnly("long-polling");
    }

    public class MetaHandshakeMessageHandlerScenario : MessageHandlerScenario<MetaHandshakeHandler>
    {
        protected static readonly string GeneratedClientID = "GeneratedClientId";

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();
            client.Stub(x => x.ID).Return(GeneratedClientID);

            Dependency<IClientIDGenerator>().Stub(x => x.GenerateClientID()).Return(GeneratedClientID);
            Dependency<IClientFactory>().Stub(x => x.CreateClient(Arg<string>.Is.Anything)).Return(client);

            eventHubMonitor.StartMonitoring<HandshakingEvent, HandshakenEvent>();
        };
    }
}
