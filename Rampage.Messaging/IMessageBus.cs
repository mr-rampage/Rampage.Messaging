using System;

namespace Rampage.Messaging
{
    public delegate void Unsubscribe();
    
    public interface IMessageBus<T>
    {
        void Publish(T message);
        Unsubscribe Subscribe(Action<T> handler);
    }
}