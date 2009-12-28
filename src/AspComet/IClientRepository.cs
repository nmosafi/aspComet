using System.Collections.Generic;

namespace AspComet
{
    public interface IClientRepository
    {
        bool Exists(string clientID);
        Client GetByID(string clientID);
        void RemoveByID(string clientID);
        void Add(Client client);
        IEnumerable<Client> WhereSubscribedTo(string channel);
    }
}