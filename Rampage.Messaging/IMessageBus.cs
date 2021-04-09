using System;

namespace Rampage.Messaging
{
    public delegate void Unsubscribe();
    public interface IMessageBus
    {
        void Publish(IMessage message);
        Unsubscribe Subscribe<T>(Action<T> handler) 
            where T : IMessage;
    }

    public interface IMessage
    {
    }
}