using System;
using System.Collections.Generic;

namespace Rampage.Messaging.Hub
{
    public class HubNode: IHub
    {
        private enum State
        {
            Started, Stopped
        }

        private State _state = State.Stopped;
        private IMessageBus _messageBus;
        private readonly List<IServiceNode> _services = new List<IServiceNode>();
        
        public IHub Deploy(IServiceNode serviceNode)
        {
            _services.Add(serviceNode);
            return this;
        }

        public IHub Undeploy(IServiceNode serviceNode)
        {
            serviceNode.Stop();
            _services.Remove(serviceNode);

            return this;
        }

        public void Start(IMessageBus messageBus)
        {
            if (_state != State.Stopped) return;
            _messageBus = messageBus;
            
            foreach (var service in _services)
                service.Start(_messageBus);
            
            _messageBus.Publish(new HubStarted());
            _state = State.Started;
        }

        public void Stop()
        {
            if (_state != State.Started) return;
            
            if (_messageBus is IDisposable disposable)
                disposable.Dispose();

            foreach (var service in _services)
                service.Stop();
            _services.Clear();
            
            _messageBus.Publish(new HubStopped());
            _state = State.Stopped;
        }
    }

    public readonly struct HubStarted : IMessage
    {
        
    }

    public readonly struct HubStopped : IMessage
    {
        
    }
}