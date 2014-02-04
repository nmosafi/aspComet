using AspComet.Eventing;

using Machine.Specifications;

using Rhino.Mocks;
using System;

namespace AspComet.Specifications
{
    [Subject(typeof(ClientWorkflowManager))]
    public class when_a_new_client_is_registered : AutoStubbingScenario<ClientWorkflowManager>
    {
        static IClient client;

        Establish context = () =>
            client = MockRepository.GenerateStub<IClient>();

        Because of = () =>
            SUT.RegisterClient(client);

        It should_insert_the_client_into_the_repository =()=>
            Dependency<IClientRepository>().ShouldHaveHadCalled(x => x.Insert(client));
    }

    [Subject(typeof(ClientWorkflowManager))]
    public class when_a_registered_client_disconnects : AutoStubbingScenario<ClientWorkflowManager>
    {
        const string ClientID = "TheClientID";
        static IClient client;
        static EventHubMonitor eventHubMonitor;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();
            client.Stub(x => x.ID).Return(ClientID);
            SUT.RegisterClient(client);
            eventHubMonitor = new EventHubMonitor();
            eventHubMonitor.StartMonitoring<DisconnectedEvent>();
        };

        Because of = () =>
            client.Raise(x => x.Disconnected += null, client, EventArgs.Empty);

        It should_publish_a_disconnected_event_with_the_client_which_disconnected =()=>
            eventHubMonitor.RaisedEvent<DisconnectedEvent>().Client.ShouldEqual(client);

        It should_delete_the_client_from_the_repository =()=>
            Dependency<IClientRepository>().ShouldHaveHadCalled(x => x.DeleteByID(ClientID));
    }

    [Subject(typeof(ClientWorkflowManager))]
    public class when_an_already_disconnected_client_disconnects : AutoStubbingScenario<ClientWorkflowManager>
    {
        const string ClientID = "TheClientID";
        static IClient client;
        static EventHubMonitor eventHubMonitor;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();
            client.Stub(x => x.ID).Return(ClientID);
            SUT.RegisterClient(client);
            eventHubMonitor = new EventHubMonitor();
            client.Raise(x => x.Disconnected += null, client, EventArgs.Empty);

            eventHubMonitor.StartMonitoring<DisconnectedEvent>();
        };

        private Because of = () => client.Raise(x => x.Disconnected += null, client, EventArgs.Empty);

        It should_not_publish_the_disconnected_event_again = () => eventHubMonitor.RaisedEvent<DisconnectedEvent>().ShouldBeNull();
    }

    public abstract class ClientWorkflowManagerScenario : AutoStubbingScenario<ClientWorkflowManager>
    {
        protected const string ClientID = "TheClientID";
        protected static IClient client;
        protected static EventHubMonitor eventHubMonitor;

        Establish context = () =>
        {
            client = MockRepository.GenerateStub<IClient>();
            client.Stub(x => x.ID).Return(ClientID);
            SUT.RegisterClient(client);
            eventHubMonitor = new EventHubMonitor();
        };
    }
}