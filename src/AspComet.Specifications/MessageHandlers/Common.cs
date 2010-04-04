// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;

using AspComet.Eventing;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    public static class Constants
    {
        public const string MessageHandlingSubject = "Message handling";
    }

    [Behaviors]
    public class ItHasHandledAMessage : MessageHandlerScenario<IMessageHandler>
    {
        It should_return_a_message_with_the_same_id_as_the_request_message = () =>
            result.Message.id.ShouldEqual(request.id);

        It should_return_a_message_with_the_same_channel_as_the_request_message = () =>
            result.Message.channel.ShouldEqual(request.channel);

        It should_return_a_message_with_the_same_clientId_as_the_request_message = () =>
            result.Message.clientId.ShouldEqual(request.clientId);
    }

    public class MessageHandlerScenario<TMessageHandler> : AutoStubbingScenario<TMessageHandler>
        where TMessageHandler : class, IMessageHandler
    {
        protected static Message request;
        protected static MessageHandlerResult result;
        protected static EventHubMonitor eventHubMonitor;
        protected static IClient client;

        Establish context = () =>
        {
            EventHub.Reset();
            eventHubMonitor = new EventHubMonitor();

            request = MessageBuilder.BuildWithRandomPropertyValues();

            client = MockRepository.GenerateStub<IClient>();
            client.Stub(x => x.ID).Return(request.clientId);
        };
    }
}
