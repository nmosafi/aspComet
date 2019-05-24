namespace AspComet
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler GetMessageHandler(string channelName);
    }
}