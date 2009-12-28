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
        public string ChannelName { get; private set; }
        public IEnumerable<Client> Recipients { get; private set; }

        public bool ShouldWait
        {
            get { return false; }
        }

        public PassThruHandler(string channelName, IEnumerable<Client> recipients)
        {
            this.ChannelName = channelName;
            this.Recipients = recipients;
        }

        public Message HandleMessage(Message request)
        {
            bool sendToSelf = false;

            var e = new PublishingEvent(request);
            EventHub.Publish(e);

            if (!e.Cancel)
            {
                Message forward = new Message
                {
                    channel = this.ChannelName,
                    data = request.data
                    // TODO What other fields should be forwarded?
                };

                foreach (Client client in this.Recipients)
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

                return sendToSelf ? forward : null;
            }

            return null; // TODO should we return some error?
        }
    }
}
