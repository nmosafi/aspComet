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

        public IClient GetByID(string clientID)
        {
            lock (syncRoot)
            {
                return Clients.Contains(clientID) ? Clients[clientID] : null;
            }
        }

        public void RemoveByID(string clientID)
        {
            lock (syncRoot)
            {
                Clients.Remove(clientID);
            }
        }

        public void Add(Client client)
        {
            lock (syncRoot)
            {
                Clients.Add(client);
            }
        }

        public IEnumerable<IClient> WhereSubscribedTo(string channel)
        {
            lock (syncRoot)
                foreach (var client in Clients)
                    if (client.IsSubscribedTo(channel))
                        yield return client;
        }

        private class KeyedClientCollection : KeyedCollection<string, IClient>
        {
            protected override string GetKeyForItem(IClient client)
            {
                return client.ID;
            }
        }
    }
}