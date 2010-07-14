using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications
{
    [Subject("Messages processor")]
    public class when_processing_two_messages_which_can_both_be_treated_as_long_polls : MessagesProcessorScenario
    {
        Because of = () =>
            messagesProcessor.Process(new[] { requestMessage1, requestMessage2 });

        Behaves_like<ItHasProcessedTwoMessages> has_processed_two_messages;

        It should_specify_not_to_send_the_result_straight_back_to_the_client = () =>
            messagesProcessor.ShouldSendResultStraightBackToClient.ShouldBeFalse();
    }

    [Subject("Messages processor")]
    public class when_processing_two_messages_where_one_cannot_be_treated_as_a_long_poll : MessagesProcessorScenario
    {
        Establish context = () =>
            messageHandlerResult1.CanTreatAsLongPoll = false;

        Because of = () =>
            messagesProcessor.Process(new[] { requestMessage1, requestMessage2 });

        Behaves_like<ItHasProcessedTwoMessages> has_processed_two_messages;

        It should_specify_to_send_the_result_straight_back_to_the_client = () =>
            messagesProcessor.ShouldSendResultStraightBackToClient.ShouldBeTrue();
    }

    [Behaviors]
    public class ItHasProcessedTwoMessages : MessagesProcessorScenario
    {
        It should_return_the_specified_response_messages = () =>
            messagesProcessor.Result.ShouldContainOnly(responseMessage1, responseMessage2);

        It should_create_a_message_handler_for_the_first_specified_channel = () =>
            messageHandlerFactory.ShouldHaveHadCalled(x => x.GetMessageHandler(SpecifiedChannel1));

        It should_create_a_message_handler_for_the_second_specified_channel = () =>
            messageHandlerFactory.ShouldHaveHadCalled(x => x.GetMessageHandler(SpecifiedChannel2));        
    }

    public abstract class MessagesProcessorScenario
    {
        protected static string SpecifiedChannel1 = "SpecifiedChannel1";
        protected static string SpecifiedChannel2 = "SpecifiedChannel2";
        protected static readonly Message requestMessage1 = new Message { channel = SpecifiedChannel1 };
        protected static readonly Message requestMessage2 = new Message { channel = SpecifiedChannel2 };
        protected static readonly Message responseMessage1 = new Message();
        protected static readonly Message responseMessage2 = new Message();

        protected static IMessageHandlerFactory messageHandlerFactory;
        protected static MessagesProcessor messagesProcessor;
        protected static IMessageHandler messageHandler;
        protected static MessageHandlerResult messageHandlerResult1;
        protected static MessageHandlerResult messageHandlerResult2;

        Establish context = () =>
        {
            messageHandlerResult1 = new MessageHandlerResult { Message = responseMessage1, CanTreatAsLongPoll = true };
            messageHandlerResult2 = new MessageHandlerResult { Message = responseMessage2, CanTreatAsLongPoll = true };

            messageHandler = MockRepository.GenerateStub<IMessageHandler>();
            messageHandler.Stub(x => x.HandleMessage(requestMessage1)).Return(messageHandlerResult1);
            messageHandler.Stub(x => x.HandleMessage(requestMessage2)).Return(messageHandlerResult2);
            
            messageHandlerFactory = MockRepository.GenerateStub<IMessageHandlerFactory>();
            messageHandlerFactory.Stub(x => x.GetMessageHandler(Arg<string>.Is.Anything)).Return(messageHandler);
            
            messagesProcessor = new MessagesProcessor(messageHandlerFactory);
        };
    }
}