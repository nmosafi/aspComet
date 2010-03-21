// ReSharper disable InconsistentNaming
using Machine.Specifications;

namespace AspComet.Specifications.MessageHandlers
{
    public static class Constants
    {
        public const string MessageHandlingSubject = "Message handling";
    }

    [Behaviors]
    public class ItHasHandledAMessage : MessageHandlerScenario
    {
        It should_return_a_message_with_the_same_id_as_the_request_message = () =>
            result.Message.id.ShouldEqual(request.id);

        It should_return_a_message_with_the_same_channel_as_the_request_message = () =>
            result.Message.channel.ShouldEqual(request.channel);

        It should_return_a_message_with_the_same_clientId_as_the_request_message = () =>
            result.Message.clientId.ShouldEqual(request.clientId);
    }

    public class MessageHandlerScenario
    {
        protected static Message request;
        protected static MessageHandlerResult result;

        Establish context = () =>
            request = MessageBuilder.BuildRandomRequest();
    }
}
