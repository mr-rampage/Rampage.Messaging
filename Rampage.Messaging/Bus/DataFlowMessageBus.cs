using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Rampage.Messaging.Bus
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

        public Unsubscribe Subscribe(Action<IMessage> handler)
        {
            var actionBlock = new ActionBlock<IMessage>(handler);
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