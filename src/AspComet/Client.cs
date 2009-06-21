using System;
using System.Collections.Generic;
using System.Timers;

using AspComet.Eventing;

namespace AspComet
{
    public class Client : IClient
    {
        private readonly List<string> subscriptions = new List<string>();
        private readonly Queue<Message> messages = new Queue<Message>();
        private readonly object syncRoot = new object();
        private readonly Timer timer = new Timer { AutoReset = false, Enabled = false, Interval = 10000 };

        public Client(string id)
        {
            this.ID = id;
            this.timer.Elapsed += HandleTimerCallback;
        }

        public string ID { get; private set; }
        public CometAsyncResult CurrentAsyncResult { get; set; }

        public void SubscribeTo(string subscription)
        {
            if (string.IsNullOrEmpty(subscription))
                throw new ArgumentException("Subscription cannot be null or empty");

            this.subscriptions.Add(subscription);
            EventHub.Publish(new SubscribedEvent(this, subscription));
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
            lock (this.syncRoot)
            {
                foreach (Message message in messages)
                {
                    this.messages.Enqueue(message);
                }
            }

            this.timer.Start();
        }

        public void FlushQueue()
        {
            this.timer.Stop();

            if (this.messages.Count > 0 && this.CurrentAsyncResult != null)
            {
                lock (syncRoot) // double checked lock
                {
                    if (this.messages.Count > 0 && this.CurrentAsyncResult != null)
                    {
                        this.CurrentAsyncResult.ResponseMessages = this.GetMessages();
                        this.CurrentAsyncResult.Complete();
                        this.CurrentAsyncResult = null;
                    }
                }
            }
        }

        public bool IsSubscribedTo(string channel)
        {
            return this.subscriptions.Contains(channel);
        }

        private IEnumerable<Message> GetMessages()
        {
            while (this.messages.Count > 0)
            {
                yield return this.messages.Dequeue();
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