using System;
using System.Collections.Generic;
using System.Timers;

namespace AspComet
{
    public class Client : IClient
    {
        private readonly List<string> subscriptions = new List<string>();
        private readonly Queue<Message> messageQueue = new Queue<Message>();
        private readonly object syncRoot = new object();
        private readonly Timer timer = new Timer { AutoReset = false, Enabled = false, Interval = CometHttpHandler.LongPollDuration };

        public Client(string id)
        {
            this.ID = id;
            this.timer.Elapsed += HandleTimerCallback;
        }

        public string ID { get; private set; }
        public CometAsyncResult CurrentAsyncResult { get; set; }
        public bool IsConnected { get; private set; }

        public void SubscribeTo(string subscription)
        {
            if (string.IsNullOrEmpty(subscription))
                throw new ArgumentException("Subscription cannot be null or empty");

            this.subscriptions.Add(subscription);
        }

        public void UnsubscribeFrom(string subscription)
        {
            if (string.IsNullOrEmpty(subscription))
                throw new ArgumentException("Subscription cannot be null or empty");

            lock (this.syncRoot)
            {
                if (!this.subscriptions.Contains(subscription))
                    throw new ArgumentException("Cannot unsubscribe when not already subscribed");

                this.subscriptions.Remove(subscription);
            }
        }

        public void Enqueue(params Message[] messages)
        {
            this.Enqueue((IEnumerable<Message>) messages);
        }

        public void Enqueue(IEnumerable<Message> messages)
        {
            lock (this.syncRoot)
            {
                foreach (Message message in messages)
                {
                    this.messageQueue.Enqueue(message);
                }
            }

            this.timer.Start();
        }

        public void FlushQueue()
        {
            this.timer.Stop();

            if (this.messageQueue.Count > 0 && this.CurrentAsyncResult != null)
            {
                lock (syncRoot) // double checked lock
                {
                    if (this.messageQueue.Count > 0 && this.CurrentAsyncResult != null)
                    {
                        IEnumerable<Message> response = this.GetMessages();
                        this.CurrentAsyncResult.CompleteRequestWithMessages(response);
                        this.CurrentAsyncResult = null;
                    }
                }
            }
        }

        public bool IsSubscribedTo(string channel)
        {
            // TODO: Change to use ChannelName class which supports wild cards
            return this.subscriptions.Contains(channel);
        }

        public void NotifyConnected()
        {
            this.IsConnected = true;
        }

        private IEnumerable<Message> GetMessages()
        {
            while (this.messageQueue.Count > 0)
            {
                yield return this.messageQueue.Dequeue();
            }
        }

        private void HandleTimerCallback(object state, ElapsedEventArgs e)
        {
            this.FlushQueue();
        }

        #region Object Members

        public bool Equals(Client obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.ID, this.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Client)) return false;
            return this.Equals((Client)obj);
        }

        public override int GetHashCode()
        {
            return (this.ID != null ? this.ID.GetHashCode() : 0);
        }

        #endregion
    }
}