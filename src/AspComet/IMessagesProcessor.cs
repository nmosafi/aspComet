using System.Collections.Generic;

namespace AspComet
{
    public interface IMessagesProcessor
    {
        bool ShouldSendResultStraightBackToClient { get; }
        IEnumerable<Message> Result { get; }
        void Process(IEnumerable<Message> messages);
    }
}