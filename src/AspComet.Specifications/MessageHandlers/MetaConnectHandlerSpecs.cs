// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_connect_message_for_an_unknown_client : MessageHandlerScenario<MetaConnectHandler>
    {
        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_return_an_unsuccessful_message = () =>
            result.Message.successful.ShouldEqual(false);

        It should_return_a_connection_type_of_long_polling = () =>
            result.Message.connectionType.ShouldEqual("long-polling");

        It should_return_a_message_with_an_error_of_clientId_not_recognised = () =>
            result.Message.error.ShouldEqual("clientId not recognised");

        It should_return_advice_to_reconnect_with_a_handshake = () =>
            result.Message.GetAdvice<string>("reconnect").ShouldEqual("handshake");

        It should_specify_that_the_response_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_connect_message_for_a_client_which_has_not_connected_before : MessageHandlerScenario<MetaConnectHandler>
    {
        Establish context =()=>
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> 
            has_handled_a_message;

        Behaves_like<ItHasSuccessfullyHandledAMetaConnectMessage> 
            has_successfully_handled_a_meta_connect_message;

        It should_specify_that_the_response_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_connect_message_for_a_client_which_has_already_connected : MessageHandlerScenario<MetaConnectHandler>
    {
        Establish context = () =>
        {
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);
            client.Stub(x => x.IsConnected).Return(true);
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage>
            has_handled_a_message;

        Behaves_like<ItHasSuccessfullyHandledAMetaConnectMessage>
            has_successfully_handled_a_meta_connect_message;

        It should_specify_that_the_response_can_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeTrue();
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_connect_message_for_a_client_which_has_already_connected_but_has_pending_messages : MessageHandlerScenario<MetaConnectHandler>
    {
        Establish context = () =>
        {
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);
            client.Stub(x => x.IsConnected).Return(true);
            client.Stub(x => x.HasPendingMessages).Return(true);
        };

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage>
            has_handled_a_message;

        Behaves_like<ItHasSuccessfullyHandledAMetaConnectMessage>
            has_successfully_handled_a_meta_connect_message;

        It should_specify_that_the_response_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }

    [Behaviors]
    public class ItHasSuccessfullyHandledAMetaConnectMessage : MessageHandlerScenario<MetaConnectHandler>
    {
        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_return_a_connection_type_of_long_polling = () =>
            result.Message.connectionType.ShouldEqual("long-polling");

        It should_return_an_advice_with_timeout_equal_to_the_configured_long_poll_duration = () =>
            result.Message.GetAdvice<int>("timeout").ShouldEqual(CometHttpHandler.LongPollDurationInMilliseconds);

        It should_notify_the_client_that_is_is_now_connected = () =>
            client.ShouldHaveHadCalled(x => x.NotifyConnected());
    }
}