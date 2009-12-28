using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspComet
{
    /// <summary>
    ///     Stores clients in memory.  All operations are locked and therefore thread safe when used independently.
    /// </summary>
    public class InMemoryClientRepository : IClientRepository
    {
        private static readonly KeyedClientCollection Clients = new KeyedClientCollection();
        private readonly object syncRoot = new object();

        public bool Exists(string clientID)
        {
            lock (syncRoot)
                return Clients.Contains(clientID);
        }

        public Client GetByID(string clientID)
        {
            lock (syncRoot)
                return Clients[clientID];
        }

        public void RemoveByID(string clientID)
        {
            lock (syncRoot)
                Clients.Remove(clientID);
        }

        public void Add(Client client)
        {
            lock (syncRoot)
                Clients.Add(client);
        }

        public IEnumerable<Client> WhereSubscribedTo(string channel)
        {
            lock (syncRoot)
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