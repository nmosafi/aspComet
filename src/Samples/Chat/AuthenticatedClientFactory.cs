namespace AspComet.Samples.Chat
{
    public class AuthenticatedClientFactory : AspComet.IClientFactory 
    {
        public Client CreateClient(string id)
        {
            return new AuthenticatedClient(id);
        }

    }
}
