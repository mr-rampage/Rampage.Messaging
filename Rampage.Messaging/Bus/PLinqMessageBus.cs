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

        public Unsubscribe Subscribe(Action<IMessage> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}