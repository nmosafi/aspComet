namespace AspComet
{
    /// <summary>
    ///     The default client factory.  Applications can provide their own factory if they wish to
    ///     have their own type of client
    /// </summary>
    public class ClientFactory : IClientFactory
    {
        public IClient CreateClient(string id)
        {
            return new Client(id);
        }
    }
}