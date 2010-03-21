using System.Collections.Generic;

namespace AspComet
{
    public interface IClientRepository
    {
        IClient GetByID(string clientID);
        void RemoveByID(string clientID);
        void Add(Client client);
        IEnumerable<IClient> WhereSubscribedTo(string channel);
    }
}