using System.Collections.Generic;
using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    /// <summary>
    ///     A message handler which passes the message through to any clients subscribed to the
    ///     relevant channel
    /// </summary>
    public class PassThruHandler : IMessageHandler
    {
        private string channelName;
        private IEnumerable<IClient> recipients;

        public PassThruHandler(string channelName, IEnumerable<IClient> recipients)
        {
            this.channelName = channelName;
            this.recipients = recipients;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            PublishingEvent e = new PublishingEvent(request);
            EventHub.Publish(e);

            if (e.Cancel)
            {
            	return new MessageHandlerResult
    	       	{
    	       		Message = new Message
   		          	{
   		          		successful = false,
   		          		channel = this.channelName,
   		          		error = e.CancellationReason
   		          	},
    	       		CanTreatAsLongPoll = false
    	       	};
            }

            Message forward = new Message
            {
                channel = this.channelName,
                data = request.data
                // TODO What other fields should be forwarded?
            };

            bool sendToSelf = false;
            foreach (Client client in this.recipients)
            {
                if (client.ID == request.clientId)
                {
                    sendToSelf = true;
                }
                else
                {
                    client.Enqueue(forward);
                    client.FlushQueue();
                }
            }

            return new MessageHandlerResult { Message = sendToSelf ? forward : null, CanTreatAsLongPoll = false };
        }
    }
}
