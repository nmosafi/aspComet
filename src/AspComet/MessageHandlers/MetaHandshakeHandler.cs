using System.Collections.Generic;

using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaHandshakeHandler : IMessageHandler
    {
        public string ChannelName
        {
            get { return "/meta/handshake"; }
        }

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(MessageBus source, Message request)
        {
            Client client = source.CreateClient();
            var e1 = new HandshakingEvent(client, request);
            EventHub.Publish(e1);
            if( e1.Cancel ) 
            {
                source.RemoveClient(client.ID);
                return GetHandshakeFailedResponse(request, client, e1.CancellationReason, e1.Retry);
            }

            var e2 = new HandshakenEvent(client);
            EventHub.Publish(e2);

            return GetHandshakeSucceededResponse(request, client);
        }

        private Message GetHandshakeSucceededResponse(Message request, IClient client)
        {
            // The handshaks success response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            return new Message
            {
                channel = this.ChannelName,
                version = "1.0",
                supportedConnectionTypes = new[] { "long-polling" },
                clientId = client.ID,
                successful = true,
                id = request.id,
            };
        }

        private Message GetHandshakeFailedResponse(Message request, IClient client, string cancellationReason, bool retry)
        {
            // The handshake failed response is documented at
            // http://svn.cometd.org/trunk/bayeux/bayeux.html#toc_50

            Dictionary<string, string> handshakeAdvice = new Dictionary<string,string>();
            if (!retry)
            {
                handshakeAdvice["reconnect"] = "none";
            }

            return new Message
            {
                channel = this.ChannelName,
                successful = false,
                error = cancellationReason,
                supportedConnectionTypes = new[] { "long-polling" },
                version = "1.0",
                id = request.id,
                advice = handshakeAdvice
            };
        }

    }
}