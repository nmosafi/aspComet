using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AspComet
{
    public class Client
    {
        private readonly List<string> subscriptions = new List<string>();
        private readonly Queue<Message> messages = new Queue<Message>();
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private readonly object syncRoot = new object();

        public Client()
        {
            this.ID = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public string ID { get; private set; }

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

        public void Enqueue(Message message)
        {
            lock (this.syncRoot)
            {
                this.messages.Enqueue(message);
                this.autoResetEvent.Set();
            }
        }

        public bool IsSubscribedTo(string channel)
        {
            return this.subscriptions.Contains(channel);
        }

        public List<Message> WaitForQueuedMessages()
        {
            if (this.subscriptions.Count > 0 && this.messages.Count == 0)
            {
                this.autoResetEvent.WaitOne(TimeSpan.FromSeconds(10));
            }
            return this.GetMessages().ToList();
        }

        private IEnumerable<Message> GetMessages()
        {
            while (this.messages.Count > 0)
            {
                yield return this.messages.Dequeue();
            }
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