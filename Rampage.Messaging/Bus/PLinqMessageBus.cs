using System;
using System.Collections.Generic;
using System.Linq;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Bus
{
    public sealed class PLinqMessageBus : IMessageBus
    {
        private readonly List<Action<IMessage>> _subscribers = new List<Action<IMessage>>();

        public void Publish(IMessage message)
        {
            foreach (var handler in _subscribers.AsParallel())
                handler.Invoke(message);
        }

        public Unsubscribe Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var f = Combinators.ApplyForType(handler);
            _subscribers.Add(f);
            return () => _subscribers.Remove(f);
        }
    }
}