using System.Collections.Generic;

namespace AspComet
{
    public interface IClientRepository
    {
        IClient GetByID(string clientID);
        void DeleteByID(string clientID);
        void Insert(IClient client);
        IEnumerable<IClient> All();
        IEnumerable<IClient> WhereSubscribedTo(string channel);
    }
}