using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Bus
{
    public sealed class ParallelMessageBus : IMessageBus
    {
        private readonly List<Action<IMessage>> _subscribers = new List<Action<IMessage>>();

        public void Publish(IMessage message)
        {
            Parallel.ForEach(_subscribers, Combinators.Thrush(message));
        }

        public Unsubscribe Subscribe(Action<IMessage> handler)
        {
            _subscribers.Add(handler);
            return () => _subscribers.Remove(handler);
        }
    }
}