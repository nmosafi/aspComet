using AspComet;
namespace AspCometCoreApp
{
    public class AuthenticatedClientFactory : AspComet.IClientFactory 
    {
        public IClient CreateClient(string id)
        {
            return new AuthenticatedClient(id);
        }

    }
}
