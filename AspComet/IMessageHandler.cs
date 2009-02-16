namespace AspComet
{
    public interface IMessageHandler
    {
        string ChannelName { get; }
        Message HandleMessage(MessageBroker source, Message request);
    }
}