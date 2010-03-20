// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications
{
    [Subject("Creating message handlers")]
    public class when_no_channel_is_given : MessageHandlerFactoryScenario
    {
        It should_create_an_exception_handler =()=>
            messageHandlerFactory.GetMessageHandler("").ShouldBeOfType<ExceptionHandler>();

        It should_create_an_exception_handler_with_message_empty_channel_field_in_request =()=>
            ((ExceptionHandler) messageHandlerFactory.GetMessageHandler("")).ErrorMessage.ShouldEqual("Empty channel field in request");
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_handshake_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_MetaHandshakeHandler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/handshake").ShouldBeOfType<MetaHandshakeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_subscribe_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_MetaSubcribeHandler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/subscribe").ShouldBeOfType<MetaSubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_unsubscribe_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_MetaUnsubcribeHandler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/unsubscribe").ShouldBeOfType<MetaUnsubscribeHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_connect_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_MetaConnectHandler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/connect").ShouldBeOfType<MetaConnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_the_meta_disconnect_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_MetaDisconnectHandler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/disconnect").ShouldBeOfType<MetaDisconnectHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_an_unknown_meta_channel : MessageHandlerFactoryScenario
    {
        It should_create_an_exception_handler = () =>
            messageHandlerFactory.GetMessageHandler("/meta/idonotexist").ShouldBeOfType<ExceptionHandler>();

        It should_create_an_exception_handler_with_message_Unknwon_meta_channel = () =>
            ((ExceptionHandler)messageHandlerFactory.GetMessageHandler("/meta/idonotexist")).ErrorMessage.ShouldEqual("Unknown meta channel.");
    }

    [Subject("Creating message handlers")]
    public class for_a_service_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_SwallowHandler = () =>
            messageHandlerFactory.GetMessageHandler("/service/fdskml").ShouldBeOfType<SwallowHandler>();
    }

    [Subject("Creating message handlers")]
    public class for_any_other_channel : MessageHandlerFactoryScenario
    {
        It should_create_a_PassThruHandler = () =>
            messageHandlerFactory.GetMessageHandler("/abc/def").ShouldBeOfType<PassThruHandler>();
    }

    public class MessageHandlerFactoryScenario
    {
        protected static MessageHandlerFactory messageHandlerFactory;

        Establish context =()=>
        {
            messageHandlerFactory = new MessageHandlerFactory(MockRepository.GenerateStub<IClientRepository>(), null, null);
        };
    }
}