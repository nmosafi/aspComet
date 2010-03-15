using System.Collections.Generic;

namespace AspComet
{
    public interface IClientRepository
    {
        bool Exists(string clientID);
        IClient GetByID(string clientID);
        void RemoveByID(string clientID);
        void Add(Client client);
        IEnumerable<IClient> WhereSubscribedTo(string channel);
    }
}