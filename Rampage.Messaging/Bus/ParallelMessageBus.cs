using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rampage.Messaging.Bus
{
    public sealed class ParallelMessageBus<T> : IMessageBus<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();

        public void Publish(T message)
        {
            Parallel.ForEach(_subscribers, action => action(message));
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}