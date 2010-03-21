namespace AspComet.Samples.Chat
{
    public class AuthenticatedClientFactory : AspComet.IClientFactory 
    {
        public IClient CreateClient(string id)
        {
            return new AuthenticatedClient(id);
        }

    }
}
