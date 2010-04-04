// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_disconnect_message : MessageHandlerScenario<MetaDisconnectHandler>
    {
        Establish context = () =>
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_tell_the_client_it_has_disconnected = () =>
            client.ShouldHaveHadCalled(x => x.Disconnect());

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
            
    }
}