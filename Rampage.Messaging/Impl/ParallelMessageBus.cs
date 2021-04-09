using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rampage.Messaging.Impl
{
    public sealed class ParallelMessageBus : IMessageBus
    {
        private readonly List<Action<IMessage>> _subscribers = new List<Action<IMessage>>();

        public void Publish(IMessage message)
        {
            Parallel.ForEach(_subscribers, Combinators.Thrush(message));
        }

        public Unsubscribe Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var f = Combinators.ApplyForType(handler);
            _subscribers.Add(f);
            return () => _subscribers.Remove(f);
        }
    }
}