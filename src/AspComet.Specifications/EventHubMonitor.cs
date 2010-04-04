using System;
using System.Collections.Generic;

using AspComet.Eventing;

namespace AspComet.Specifications
{
    public class EventHubMonitor
    {
        private readonly Dictionary<Type, IEvent> events = new Dictionary<Type, IEvent>();

        public void StartMonitoring<T>() where T : IEvent
        {
            EventHub.Subscribe<T>(ev => events[typeof(T)] = ev);
        }

        public void StartMonitoring<T1, T2>()
            where T1 : IEvent
            where T2 : IEvent
        {
            StartMonitoring<T1>();
            StartMonitoring<T2>();
        }

        public T RaisedEvent<T>() where T : IEvent
        {
            Type type = typeof(T);
            return events.ContainsKey(type) ? (T)events[type] : default(T);
        }
    }
}