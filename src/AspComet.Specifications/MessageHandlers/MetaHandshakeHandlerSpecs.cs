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
            result = metaHandshakeHandler.HandleMessage(request);

        Behaves_like<ItHasHandledAMetaHandshakeMessage> has_handled_a_meta_handshake_message;

        It should_register_the_client_with_the_client_workflow_manager = () =>
            clientWorkflowManager.ShouldHaveHadCalled(x => x.RegisterClient(clientWhichWasCreated));

        It should_publish_a_handshaken_event_with_the_client_which_was_created = () =>
            handshakenEventWhichWasRaised.Client.ShouldEqual(clientWhichWasCreated);

        It should_return_a_successul_message = () =>
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
            result = metaHandshakeHandler.HandleMessage(request);

        Behaves_like<ItHasHandledAMetaHandshakeMessage> has_handled_a_meta_handshake_message;

        It should_not_register_the_client_with_the_client_workflow_manager = () =>
            clientWorkflowManager.ShouldNotHaveHadCalled(x => x.RegisterClient(Arg<IClient>.Is.Anything));

        It should_not_publish_a_handshaken_event = () =>
            handshakenEventWhichWasRaised.ShouldBeNull();

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
            clientFactory.ShouldHaveHadCalled(x => x.CreateClient(GeneratedClientID));

        It should_publish_a_handshaking_event_with_the_client_which_was_created = () =>
            handshakingEventWhichWasRaised.Client.ShouldEqual(clientWhichWasCreated);

        It should_publish_a_handshaking_event_with_the_request_message = () =>
            handshakingEventWhichWasRaised.Handshake.ShouldEqual(request);

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

    public class MetaHandshakeMessageHandlerScenario : MessageHandlerScenario
    {
        protected static readonly string GeneratedClientID = "GeneratedClientId";
        protected static IClientIDGenerator clientIDGenerator;
        protected static IClientFactory clientFactory;
        protected static IClientWorkflowManager clientWorkflowManager;
        protected static MetaHandshakeHandler metaHandshakeHandler;
        protected static IClient clientWhichWasCreated;
        protected static HandshakingEvent handshakingEventWhichWasRaised;
        protected static HandshakenEvent handshakenEventWhichWasRaised;

        Establish context = () =>
        {
            clientIDGenerator = MockRepository.GenerateStub<IClientIDGenerator>();
            clientIDGenerator.Stub(x => x.GenerateClientID()).Return(GeneratedClientID);

            clientWhichWasCreated = MockRepository.GenerateStub<IClient>();
            clientWhichWasCreated.Stub(x => x.ID).Return(GeneratedClientID);

            clientFactory = MockRepository.GenerateStub<IClientFactory>();
            clientFactory.Stub(x => x.CreateClient(Arg<string>.Is.Anything)).Return(clientWhichWasCreated);

            clientWorkflowManager = MockRepository.GenerateStub<IClientWorkflowManager>();

            metaHandshakeHandler = new MetaHandshakeHandler(clientIDGenerator, clientFactory, clientWorkflowManager);

            handshakingEventWhichWasRaised = null;
            handshakenEventWhichWasRaised = null;
            EventHub.Subscribe<HandshakingEvent>(ev => handshakingEventWhichWasRaised = ev);
            EventHub.Subscribe<HandshakenEvent>(ev => handshakenEventWhichWasRaised = ev);
        };
    }
}
