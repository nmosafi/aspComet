// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

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
        It should_create_a_MetaHandshakeHandler = () =>
            SUT.GetMessageHandler("/meta/handshake").ShouldBeOfType<MetaHandshakeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_subscribe_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_MetaSubcribeHandler = () =>
            SUT.GetMessageHandler("/meta/subscribe").ShouldBeOfType<MetaSubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_unsubscribe_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_MetaUnsubcribeHandler = () =>
            SUT.GetMessageHandler("/meta/unsubscribe").ShouldBeOfType<MetaUnsubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_connect_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_MetaConnectHandler = () =>
            SUT.GetMessageHandler("/meta/connect").ShouldBeOfType<MetaConnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_disconnect_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_MetaDisconnectHandler = () =>
            SUT.GetMessageHandler("/meta/disconnect").ShouldBeOfType<MetaDisconnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_an_unknown_meta_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_an_exception_handler = () =>
            SUT.GetMessageHandler("/meta/idonotexist").ShouldBeOfType<ExceptionHandler>();

        It should_create_an_exception_handler_with_message_Unknwon_meta_channel = () =>
            ((ExceptionHandler)SUT.GetMessageHandler("/meta/idonotexist")).ErrorMessage.ShouldEqual("Unknown meta channel.");
    }

    [Subject("Creating message handlers")]
    public class for_a_service_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_SwallowHandler = () =>
            SUT.GetMessageHandler("/service/fdskml").ShouldBeOfType<SwallowHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_any_other_channel : AutoStubbingScenario<MessageHandlerFactory>
    {
        It should_create_a_PassThruHandler = () =>
            SUT.GetMessageHandler("/abc/def").ShouldBeOfType<ForwardingHandler>();
    }
}