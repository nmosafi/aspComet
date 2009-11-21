using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspComet
{
    public class InMemoryClientRepository : IClientRepository
    {
        private static readonly KeyedClientCollection Clients = new KeyedClientCollection();

        public bool ContainsID(string clientID)
        {
            return Clients.Contains(clientID);
        }

        public Client GetByID(string clientID)
        {
            return Clients[clientID];
        }

        public void RemoveByID(string clientID)
        {
            Clients.Remove(clientID);
        }

        public void Add(Client client)
        {
            Clients.Add(client);
        }

        public IEnumerable<Client> WhereSubscribedTo(string channel)
        {
            foreach (var client in Clients)
                if (client.IsSubscribedTo(channel))
                    yield return client;
        }

        private class KeyedClientCollection : KeyedCollection<string, Client>
        {
            protected override string GetKeyForItem(Client client)
            {
                return client.ID;
            }
        }
    }
}