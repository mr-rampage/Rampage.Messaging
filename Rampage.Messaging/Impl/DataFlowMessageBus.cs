using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Rampage.Messaging.Impl
{
    public sealed class DataFlowMessageBus : IMessageBus, IDisposable
    {
        private readonly IPropagatorBlock<IMessage, IMessage> _mailBox;
        private readonly IPropagatorBlock<IMessage, IMessage> _broadcast;
        private readonly List<IDisposable> _cleanup = new List<IDisposable>();

        public DataFlowMessageBus()
        {
            _broadcast = new BroadcastBlock<IMessage>(null);
            _mailBox = new BufferBlock<IMessage>();
            _cleanup.Add(_mailBox.LinkTo(_broadcast, new DataflowLinkOptions
            {
                PropagateCompletion = true
            }));
        }

        public void Publish(IMessage message)
        {
            _mailBox.SendAsync(message);
        }

        public Unsubscribe Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var actionBlock = new ActionBlock<IMessage>(message => handler((T) message));
            var disposable = _broadcast.LinkTo(actionBlock, message => message.GetType() == typeof(T));
            _cleanup.Add(disposable);
            return () =>
            {
                disposable.Dispose();
                _cleanup.Remove(disposable);
            };
        }

        public void Dispose()
        {
            _mailBox.Complete();
            foreach (var disposable in _cleanup)
            {
                disposable.Dispose();
            }
        }
    }
}