using System;

namespace AspComet
{
    public interface ICometAsyncResult : IAsyncResult
    {
        void SendAwaitingMessages();
    }
}