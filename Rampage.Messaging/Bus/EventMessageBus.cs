using System;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Bus
{
    public sealed class EventMessageBus: IMessageBus, IDisposable
    {
        private delegate void MessageEventHandler(IMessage e);

        private event MessageEventHandler OnMessageHandler;

        public void Publish(IMessage message)
        {
            OnMessageHandler?.Invoke(message);
        }

        public Unsubscribe Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var f = Combinators.ApplyForType(handler);
            void EventHandler(IMessage e) => f(e);
            OnMessageHandler += EventHandler;
            return () => OnMessageHandler -= EventHandler;
        }

        public void Dispose()
        {
            foreach (var @delegate in OnMessageHandler?.GetInvocationList() ?? new Delegate[] {})
            {
                OnMessageHandler -= @delegate as MessageEventHandler;
            }
        }
    }
}