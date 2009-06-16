namespace AspComet
{
    public interface IMessageBus
    {
        void HandleMessages(Message[] messages, CometAsyncResult cometAsyncResult);
    }
}