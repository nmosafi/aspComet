using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        public void DeleteByID(string clientID)
        {
            lock (syncRoot)
            {
                Clients.Remove(clientID);
            }
        }

        public void Insert(IClient client)
        {
            lock (syncRoot)
            {
                Clients.Add(client);
            }
        }

        public IEnumerable<IClient> All()
        {
            lock (syncRoot)
            {
                return Clients.ToList();
            }
        }

        public IEnumerable<IClient> WhereSubscribedTo(string channel)
        {
            lock (syncRoot)
            {
                return Clients.Where(client => client.IsSubscribedTo(channel)).ToList(); // yield within a lock... urgh
            }
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