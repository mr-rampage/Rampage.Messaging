using System;
using System.Collections.Generic;
using System.Linq;

namespace Rampage.Messaging.Bus
{
    public sealed class PLinqMessageBus<T> : IMessageBus<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();

        public void Publish(T message)
        {
            foreach (var handler in _subscribers.AsParallel())
                handler.Invoke(message);
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}