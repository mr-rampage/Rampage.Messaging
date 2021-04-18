using System;
using System.Collections.Generic;

namespace Rampage.Messaging.Hub
{
    public class HubNode<T>: IHub<T>
    {
        private enum State
        {
            Started, Stopped
        }

        private State _state = State.Stopped;
        private IMessageBus<T> _messageBus;
        private readonly List<IServiceNode<T>> _services = new List<IServiceNode<T>>();
        
        public IHub<T> Deploy(IServiceNode<T> serviceNode)
        {
            _services.Add(serviceNode);

            if (_state == State.Started)
                serviceNode.Start(_messageBus);
            
            return this;
        }

        public IHub<T> Undeploy(IServiceNode<T> serviceNode)
        {
            serviceNode.Stop();
            
            if (_services.Contains(serviceNode))
                _services.Remove(serviceNode);

            return this;
        }

        public void Start(IMessageBus<T> messageBus)
        {
            if (_state != State.Stopped) return;
            _messageBus = messageBus;
            
            foreach (var service in _services)
                service.Start(_messageBus);
            
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
            
            _state = State.Stopped;
        }
    }
}