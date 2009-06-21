namespace AspComet
{
    public interface IClient
    {
        void Enqueue(params Message[] messages);
    }
}