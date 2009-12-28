namespace AspComet
{
    public interface IMessageHandler
    {
        string ChannelName { get; }
        bool ShouldWait { get; }
        Message HandleMessage(Message request);
    }
}