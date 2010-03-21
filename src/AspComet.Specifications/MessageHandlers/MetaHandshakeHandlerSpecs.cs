// ReSharper disable InconsistentNaming

using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_handhake_message : MetaHandshakeMessageHandlerScenario
    {
        Because of =()=>
            result = metaHandshakeHandler.HandleMessage(request);

        It should_create_a_client_with_the_id_generated_by_the_client_id_generator = () =>
            clientFactory.ShouldHaveHadCalled(x => x.CreateClient(GeneratedClientID));

        It should_publish_a_handshaking_event_with_the_client_which_was_created = () =>
            handshakingEventWhichWasRaised.Client.ShouldEqual(clientWhichWasCreated);

        It should_publish_a_handshaking_event_with_the_handshaking_message = () =>
            handshakingEventWhichWasRaised.Handshake.ShouldEqual(request);

        It should_register_the_client_with_the_client_workflow_manager = () =>
            clientWorkflowManager.ShouldHaveHadCalled(x => x.RegisterClient(clientWhichWasCreated));

        It should_publish_a_handshaken_event_with_the_client_which_was_created = () =>
            handshakenEventWhichWasRaised.Client.ShouldEqual(clientWhichWasCreated);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();

        It should_return_a_message_with_the_same_id_as_the_request_message = () =>
            result.Message.id.ShouldEqual(request.id);

        It should_return_a_message_with_the_same_channel_as_the_request_message = () =>
            result.Message.channel.ShouldEqual(request.channel);

        It should_return_a_message_with_a_client_id_equal_to_the_generated = () =>
            result.Message.clientId.ShouldEqual(GeneratedClientID);
    }

    public class when_handling_a_handshake_message_which_has_been_cancelled
    {
    }

    public class MetaHandshakeMessageHandlerScenario : MessageHandlerScenario
    {
        protected const string GeneratedClientID = "GeneratedClientId";
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

            EventHub.Subscribe<HandshakingEvent>(ev => handshakingEventWhichWasRaised = ev);
            EventHub.Subscribe<HandshakenEvent>(ev => handshakenEventWhichWasRaised = ev);
        };
    }
}
