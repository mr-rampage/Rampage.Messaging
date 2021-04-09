using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rampage.Messaging.Impl
{
    public sealed class TaskMessageBus : IMessageBus
    {
        private readonly List<Action<IMessage>> _subscribers = new List<Action<IMessage>>();
        private Task _lastTask = Task.CompletedTask;

        public void Publish(IMessage message)
        {
            _lastTask = _lastTask.ContinueWith(_ => Task.WhenAll(_subscribers.Select(Combinators.ThrushAsync(message))).Wait());
        }

        public Unsubscribe Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var f = Combinators.ApplyForType(handler);
            _subscribers.Add(f);
            return () => _subscribers.Remove(f);
        }
    }
}