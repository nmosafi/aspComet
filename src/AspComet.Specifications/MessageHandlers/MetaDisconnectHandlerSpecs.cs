// ReSharper disable InconsistentNaming

using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject("Meta disconnect handler")]
    public class when_handling_a_disconnect_message : MessageHandlerScenario
    {
        static IClient client;
        static IClientRepository clientRepository;
        static MetaDisconnectHandler metaDisconnectHandler;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();

            clientRepository = MockRepository.GenerateStub<IClientRepository>();
            clientRepository.Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);
            
            metaDisconnectHandler = new MetaDisconnectHandler(clientRepository);
        };

        Because of = () =>
            result = metaDisconnectHandler.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_tell_the_client_it_has_disconnected = () =>
            client.ShouldHaveHadCalled(x => x.Disconnect());

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();
            
    }
}