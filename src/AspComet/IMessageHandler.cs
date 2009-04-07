namespace AspComet
{
    public interface IMessageHandler
    {
        string ChannelName { get; }
        bool ShouldWait { get; }
        Message HandleMessage(MessageBus source, Message request);
    }
}