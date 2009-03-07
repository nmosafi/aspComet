namespace AspComet
{
    public interface IMessageHandler
    {
        string ChannelName { get; }
        bool ShouldWait { get; }
        Message HandleMessage(MessageBroker source, Message request);
    }
}