using System;

namespace Rampage.Messaging
{
    public delegate void Unsubscribe();
    
    public interface IMessageBus
    {
        void Publish(IMessage message);
        Unsubscribe Subscribe(Action<IMessage> handler);
    }

    public interface IMessage
    {
    }
}