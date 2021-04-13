using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Bus
{
    public sealed class TaskMessageBus : IMessageBus
    {
        private readonly List<Action<IMessage>> _subscribers = new List<Action<IMessage>>();
        private Task _lastTask = Task.CompletedTask;

        public void Publish(IMessage message)
        {
            _lastTask = _lastTask.ContinueWith(_ => Task.WhenAll(_subscribers.Select(Combinators.ThrushAsync(message))).Wait());
        }

        public Unsubscribe Subscribe(Action<IMessage> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}