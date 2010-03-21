using System;
using System.Collections.Generic;

namespace AspComet
{
    // TODO: Massive interface.  Not sure what needs to be done to sort this out yet, but it's become the biggest smell of AspComet. Think about ISP more
    public interface IClient
    {
        ICometAsyncResult CurrentAsyncResult { get; set; }
        string ID { get; }
        bool IsConnected { get; }

        void Enqueue(IEnumerable<Message> messages);
        void FlushQueue();
        bool IsSubscribedTo(string channel);
        void UnsubscribeFrom(string subscription);
        void NotifyConnected();
        void Disconnect();
        void SubscribeTo(string subscription);
        event EventHandler<EventArgs> Disconnected;
    }

    public static class ClientExtensions
    {
        public static void Enqueue(this IClient client, params Message[] messages)
        {
            client.Enqueue(messages);
        }
    }
}