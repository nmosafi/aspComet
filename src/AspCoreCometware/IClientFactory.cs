namespace AspComet
{
    /// <summary>
    ///     Provides an interface for creating clients
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        ///     Creates a client with the specified id
        /// </summary>
        /// <param name="id">The id of the client</param>
        /// <returns>The created client</returns>
        IClient CreateClient(string id);
    }
}