using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rampage.Messaging.Bus
{
    public sealed class TaskMessageBus<T> : IMessageBus<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();
        private Task _lastTask = Task.CompletedTask;

        public void Publish(T message)
        {
            _lastTask = _lastTask.ContinueWith(_ => Task.WhenAll(_subscribers.Select(action => Task.Run(() => action(message)))).Wait());
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}