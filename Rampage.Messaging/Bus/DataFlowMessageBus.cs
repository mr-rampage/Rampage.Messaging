using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Rampage.Messaging.Bus
{
    public sealed class DataFlowMessageBus<T> : IMessageBus<T>, IDisposable
    {
        private readonly IPropagatorBlock<T, T> _mailBox;
        private readonly IPropagatorBlock<T, T> _broadcast;
        private readonly List<IDisposable> _cleanup = new List<IDisposable>();

        public DataFlowMessageBus()
        {
            _broadcast = new BroadcastBlock<T>(null);
            _mailBox = new BufferBlock<T>();
            _cleanup.Add(_mailBox.LinkTo(_broadcast, new DataflowLinkOptions
            {
                PropagateCompletion = true
            }));
        }

        public void Publish(T message)
        {
            _mailBox.SendAsync(message);
        }

        public Unsubscribe Subscribe(Action<T> handler)
        {
            var actionBlock = new ActionBlock<T>(handler);
            var disposable = _broadcast.LinkTo(actionBlock);
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