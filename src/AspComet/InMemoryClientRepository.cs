using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspComet
{
    public class InMemoryClientRepository : IClientRepository
    {
        private static readonly KeyedClientCollection clients = new KeyedClientCollection();

        public bool ContainsID(string clientID)
        {
            return clients.Contains(clientID);
        }

        public Client GetByID(string clientID)
        {
            return clients[clientID];
        }

        public void RemoveByID(string clientID)
        {
            clients.Remove(clientID);
        }

        public void Add(Client client)
        {
            clients.Add(client);
        }

        public IEnumerable<Client> WhereSubscribedTo(string channel)
        {
            foreach (var client in clients)
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