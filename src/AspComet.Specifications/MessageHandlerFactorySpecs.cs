using AspComet.MessageHandlers;

using Machine.Specifications;

namespace AspComet.Specifications
{
    [Subject("Creating message handlers")]
    public class when_no_channel_is_given : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_an_exception_handler =()=>
            SUT.GetMessageHandler("").ShouldBeOfType<ExceptionHandler>();

        It should_create_an_exception_handler_with_message_empty_channel_field_in_request =()=>
            ((ExceptionHandler)SUT.GetMessageHandler("")).ErrorMessage.ShouldEqual("Empty channel field in request");
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_handshake_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_meta_handshake_handler = () =>
            SUT.GetMessageHandler("/meta/handshake").ShouldBeOfType<MetaHandshakeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_subscribe_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_meta_subcribe_handler = () =>
            SUT.GetMessageHandler("/meta/subscribe").ShouldBeOfType<MetaSubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_unsubscribe_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_meta_unsubcribe_handler = () =>
            SUT.GetMessageHandler("/meta/unsubscribe").ShouldBeOfType<MetaUnsubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_connect_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_meta_connect_handler = () =>
            SUT.GetMessageHandler("/meta/connect").ShouldBeOfType<MetaConnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_disconnect_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_meta_disconnect_handler = () =>
            SUT.GetMessageHandler("/meta/disconnect").ShouldBeOfType<MetaDisconnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_an_unknown_meta_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_an_exception_handler = () =>
            SUT.GetMessageHandler("/meta/idonotexist").ShouldBeOfType<ExceptionHandler>();

        It should_create_an_exception_handler_with_message_unknown_meta_channel = () =>
            ((ExceptionHandler)SUT.GetMessageHandler("/meta/idonotexist")).ErrorMessage.ShouldEqual("Unknown meta channel.");
    }

    [Subject("Creating message handlers")]
    public class for_a_service_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_swallow_handler = () =>
            SUT.GetMessageHandler("/service/fdskml").ShouldBeOfType<SwallowHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_any_other_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_pass_thru_handler = () =>
            SUT.GetMessageHandler("/abc/def").ShouldBeOfType<ForwardingHandler>();
    }
}