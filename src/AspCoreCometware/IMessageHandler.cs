namespace AspComet
{
    public interface IMessageHandler
    {
        MessageHandlerResult HandleMessage(Message request);
    }
}