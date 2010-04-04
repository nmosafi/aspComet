// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;

using AspComet.Eventing;

using Machine.Specifications;

namespace AspComet.Specifications.MessageHandlers
{
    public static class Constants
    {
        public const string MessageHandlingSubject = "Message handling";
    }

    public class EventHubMonitor
    {
        private static readonly Dictionary<Type, IEvent> events = new Dictionary<Type, IEvent>();

        public static void Monitor<T>() where T : IEvent
        {
            EventHub.Subscribe<T>(ev => events.Add(typeof(T), ev));
        }

        public static T PublishedEvent<T>() where T : IEvent
        {
            Type type = typeof(T);
            return events.ContainsKey(type) ? (T) events[type] : default(T);
        }
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
