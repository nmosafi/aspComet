using System;

namespace AspComet.Eventing
{
    public static class EventHub
    {
        private static readonly Lookup<Type, Delegate> Subscriptions = new Lookup<Type, Delegate>();

        public static void Subscribe<T>(Action<T> action) where T : IEvent
        {
            Subscriptions.Add(typeof(T), action);
        }

        public static void Publish<T>(T ev) where T : IEvent
        {
            // NOTE: This should be thread safe as we can't currently unsubscribe, no need to lock
            if (!Subscriptions.Contains(typeof(T)))
                return;

            foreach (Action<T> action in Subscriptions[typeof(T)])
            {
                TryAndInvoke(action, ev);
                if (ev.Cancel) return;
            }
        }

        private static void TryAndInvoke<T>(Action<T> action, T ev) where T : IEvent
        {
            try
            {
                action(ev);
            }
            catch
            {
                ; // TODO: What if one of the handlers throws an exception?
            }
        }
    }
}