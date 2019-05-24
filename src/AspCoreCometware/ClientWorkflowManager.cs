using System;

using AspComet.Eventing;

namespace AspComet
{
    public class ClientWorkflowManager : IClientWorkflowManager
    {
        private readonly IClientRepository clientRepository;

        public ClientWorkflowManager(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public void RegisterClient(IClient client)
        {
            this.clientRepository.Insert(client);
            client.Disconnected += HandleClientDisconnected;
        }

        private void HandleClientDisconnected(object sender, EventArgs e)
        {
            IClient client = (IClient) sender;
            client.Disconnected -= HandleClientDisconnected;

            clientRepository.DeleteByID(client.ID);

            DisconnectedEvent disconnectedEvent = new DisconnectedEvent(client);
            EventHub.Publish(disconnectedEvent);
        }
    }
}