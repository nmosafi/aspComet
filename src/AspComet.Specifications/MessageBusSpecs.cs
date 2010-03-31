// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications
{
    [Subject("The message bus")]
    public class when_handling_messages_with_different_client_ids : MessageBusScenario
    {
        static readonly Message[] messages = new[] { new Message { clientId = "Id1" }, new Message { clientId = "Id2" } };

        Because of = () =>
            exceptionWhichWasThrown = Catch.Exception(() => SUT.HandleMessages(messages, specifiedAsyncResult));

        It should_fail = () =>
            exceptionWhichWasThrown.ShouldNotBeNull();

        It should_fail_due_to_messages_not_having_the_same_client = () =>
            exceptionWhichWasThrown.ShouldContainErrorMessage("All messages must have the same client");
    }

    [Subject("The message bus")]
    public class when_handling_messages_without_a_client_id : MessageBusScenario
    {
        static readonly Message[] messages = new[] { new Message(), new Message() };

        Because of = () =>
            SUT.HandleMessages(messages, specifiedAsyncResult);

        It should_just_complete_the_async_result_sending_back_the_processed_messages =()=>
            specifiedAsyncResult.AssertWasCalled(x => x.CompleteRequestWithMessages(responseMessages));
    }

    [Subject("The message bus")]
    public class when_handling_messages_and_there_is_no_AsyncResult_on_client_and_it_should_not_send_back_result_immediately : MessageBusScenario
    {
        static readonly Message[] messages = new[] { new Message { clientId = SpecifiedClientId }, new Message { clientId = SpecifiedClientId } };

        Because of = () =>
            SUT.HandleMessages(messages, specifiedAsyncResult);

        Behaves_like<ItHasHandledTheMessages> has_handled_the_messages;

        It should_not_flush_the_queue_on_the_client = () =>
            client.AssertWasNotCalled(x => x.FlushQueue());
    }

    [Subject("The message bus")]
    public class when_handling_messages_and_there_is_no_AsyncResult_on_client_and_it_should_send_back_result_immediately : MessageBusScenario
    {
        static readonly Message[] messages = new[] { new Message { clientId = SpecifiedClientId }, new Message { clientId = SpecifiedClientId } };

        Establish contaxt = () =>
            messagesProcessor.Stub(x => x.ShouldSendResultStraightBackToClient).Return(true);

        Because of = () =>
            SUT.HandleMessages(messages, specifiedAsyncResult);

        Behaves_like<ItHasHandledTheMessages> has_handled_the_messages;

        It should_flush_the_queue_on_the_client_once = () =>
            client.AssertWasCalled(x => x.FlushQueue(), x => x.Repeat.Once());
    }

    [Subject("The message bus")]
    public class when_handling_messages_and_there_is_already_an_AsyncResult_on_client_and_it_should_send_back_result_immediately : MessageBusScenario
    {
        static readonly Message[] messages = new[] { new Message { clientId = SpecifiedClientId }, new Message { clientId = SpecifiedClientId } };

        Establish contaxt = () =>
        {
            client.CurrentAsyncResult = MockRepository.GenerateStub<ICometAsyncResult>();
            messagesProcessor.Stub(x => x.ShouldSendResultStraightBackToClient).Return(true);
        };

        Because of = () =>
            SUT.HandleMessages(messages, specifiedAsyncResult);

        Behaves_like<ItHasHandledTheMessages> has_handled_the_messages;

        It should_flush_the_queue_on_the_client_twice = () =>
            client.ShouldHaveHadCalled(x => x.FlushQueue(), x => x.Repeat.Twice());
    }

    [Behaviors]
    public class ItHasHandledTheMessages : MessageBusScenario
    {
        It should_retrieve_the_client_with_the_specified_id = () =>
            Dependency<IClientRepository>().ShouldHaveHadCalled(x => x.GetByID(SpecifiedClientId));

        It should_set_the_async_result_on_the_client_to_the_specified_one = () =>
            client.CurrentAsyncResult.ShouldEqual(specifiedAsyncResult);

        It should_enqueue_the_response_messages_to_the_client = () =>
            client.ShouldHaveHadCalled(x => x.Enqueue(messagesProcessor.Result));
    }


    public abstract class MessageBusScenario : AutoStubbingScenario<MessageBus>
    {
        protected static readonly string SpecifiedClientId = "SpecClientID";
        protected static readonly IEnumerable<Message> responseMessages = new Message[0];
        protected static IMessagesProcessor messagesProcessor;
        protected static ICometAsyncResult specifiedAsyncResult;
        protected static Exception exceptionWhichWasThrown;
        protected static IClient client;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

            messagesProcessor = MockRepository.GenerateStub<IMessagesProcessor>();
            messagesProcessor.Stub(x => x.Result).Return(responseMessages);
            SetDependency<Func<IMessagesProcessor>>(() => messagesProcessor);

            specifiedAsyncResult = MockRepository.GenerateStub<ICometAsyncResult>();
        };
    }
}