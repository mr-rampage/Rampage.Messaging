using System;

namespace Rampage.Messaging.Bus
{
    public sealed class EventMessageBus<T>: IMessageBus<T>, IDisposable
    {
        private delegate void MessageEventHandler(T e);

        private event MessageEventHandler OnMessageHandler;

        public void Publish(T message)
        {
            OnMessageHandler?.Invoke(message);
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            void EventHandler(T e) => handler(e);
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