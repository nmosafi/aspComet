using System.Collections;

namespace AspComet
{
    public interface IClientRepository
    {
        Client GetByID(string clientID);
        void RemoveByID(string clientID);
        void Add(Client client);
        IEnumerable WhereSubscribedTo(string channel);
    }
}