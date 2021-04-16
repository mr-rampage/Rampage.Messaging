using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Bus
{
    public sealed class ParallelMessageBus<T> : IMessageBus<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();

        public void Publish(T message)
        {
            Parallel.ForEach(_subscribers, Combinators.Thrush(message));
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}