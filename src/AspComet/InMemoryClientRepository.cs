using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace AspComet
{
    public class InMemoryClientRepository : IClientRepository
    {
        private static readonly KeyedClientCollection clients = new KeyedClientCollection();

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

        public IEnumerable WhereSubscribedTo(string channel)
        {
            return clients.Where(c => c.IsSubscribedTo(channel));
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