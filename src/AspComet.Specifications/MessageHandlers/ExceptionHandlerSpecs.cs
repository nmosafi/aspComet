// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject("Exception handler")]
    public class when_handling_a_message : MessageHandlerScenario
    {
        const string ErrorMessage = "This is the error message";
        static ExceptionHandler exceptionHandler;

        Establish context = () =>
            exceptionHandler = new ExceptionHandler(ErrorMessage);

        Because of = () =>
            result = exceptionHandler.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage>
            has_handled_a_message;

        It should_return_a_message_with_the_specified_error_message = () =>
            result.Message.error.ShouldEqual(ErrorMessage);

        It should_specify_that_the_response_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }
}