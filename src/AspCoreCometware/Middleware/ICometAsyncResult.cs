using System;
using System.Collections.Generic;

namespace AspComet
{
    public interface ICometAsyncResult : IAsyncResult
    {
        void SendAwaitingMessages();
        void CompleteRequestWithMessages(IEnumerable<Message> result);
    }
}