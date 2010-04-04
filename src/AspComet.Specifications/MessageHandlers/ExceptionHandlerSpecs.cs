// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_message_which_has_generated_an_exception : MessageHandlerScenario<ExceptionHandler>
    {
        const string ErrorMessage = "This is the error message";

        Establish context = () =>
            SetSUT(new ExceptionHandler(ErrorMessage));

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage>
            has_handled_a_message;

        It should_return_a_message_with_the_specified_error_message = () =>
            result.Message.error.ShouldEqual(ErrorMessage);

        It should_specify_that_the_response_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
    }
}