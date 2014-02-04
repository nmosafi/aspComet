// ReSharper disable InconsistentNaming

using System;
using AspComet.Eventing;
using AspComet.MessageHandlers;

using Machine.Specifications;

using Rhino.Mocks;

namespace AspComet.Specifications.MessageHandlers
{
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_unsubscribe_message_from_an_non_existent_client : MessageHandlerScenario<MetaUnsubscribeHandler>
    {
		Establish context = () =>
		{
			Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(null);

			eventHubMonitor.StartMonitoring<UnsubscribedEvent>();
		};
        Because of = () => 
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_return_an_unsuccessful_message = () =>
            result.Message.successful.ShouldEqual(false);

		It should_return_a_message_with_402_error_containing_the_client_and_that_it_is_unknown = () =>
			result.Message.error.ShouldEqual(string.Format("402:{0}:Unknown Client ID", request.clientId));
    }
    
    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_unsubscribe_message_from_a_client_which_is_not_suscribed_to_the_channel : MetaUnsubscribeHandlerScenario
    {
        Because of = () => 
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_return_an_unsuccessful_message = () =>
            result.Message.successful.ShouldEqual(false);
    }

    [Subject(Constants.MessageHandlingSubject)]
    public class when_handling_a_meta_unsubscribe_message : MetaUnsubscribeHandlerScenario
    {
        Establish context = () =>
            client.Stub(x => x.IsSubscribedTo(request.subscription)).Return(true);

        Because of = () =>
            result = SUT.HandleMessage(request);

        Behaves_like<ItHasHandledAMessage> has_handled_a_message;

        It should_retrieve_the_client_using_the_client_id_in_the_message = () =>
            Dependency<IClientRepository>().ShouldHaveHadCalled(x => x.GetByID(request.clientId));

        It should_specify_that_the_result_cannot_be_treated_as_a_long_poll = () =>
            result.CanTreatAsLongPoll.ShouldBeFalse();

        It should_unsubscribe_the_client_from_the_channel = () =>
            client.ShouldHaveHadCalled(x => x.UnsubscribeFrom(request.subscription));

        It should_publish_an_unsubscribed_event_with_the_client_which_sent_the_message = () =>
            eventHubMonitor.RaisedEvent<UnsubscribedEvent>().Client.ShouldEqual(client);

        It should_publish_an_unsubscribed_event_with_the_channel_being_unsubscribed_from = () =>
            eventHubMonitor.RaisedEvent<UnsubscribedEvent>().Channel.ShouldEqual(request.subscription);

        It should_return_a_successful_message = () =>
            result.Message.successful.ShouldEqual(true);

        It should_return_a_message_with_the_subscription_of_the_channel_being_unsubscribed_from = () =>
            result.Message.subscription.ShouldEqual(request.subscription);
    }

    public abstract class MetaUnsubscribeHandlerScenario : MessageHandlerScenario<MetaUnsubscribeHandler>
    {
        Establish context = () =>
        {
            Dependency<IClientRepository>().Stub(x => x.GetByID(Arg<string>.Is.Anything)).Return(client);

            eventHubMonitor.StartMonitoring<UnsubscribedEvent>();
        };
    }

}