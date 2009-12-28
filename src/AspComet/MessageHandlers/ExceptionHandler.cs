using System;

namespace AspComet.MessageHandlers
{
    public class ExceptionHandler : IMessageHandler
    {
        public string ChannelName { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool ShouldWait
        {
            get { return false; }
        }

        public ExceptionHandler(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public Message HandleMessage(Message request)
        {
            return new Message
            {
                id = request.id,
                channel = this.ChannelName,
                error = this.ErrorMessage
            };
        }
    }
}