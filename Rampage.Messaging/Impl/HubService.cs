using System;
using System.Collections.Generic;

namespace Rampage.Messaging.Impl
{
    public class HubService: IHub
    {
        private enum State
        {
            Started, Stopped
        }

        private State _state = State.Stopped;
        private IMessageBus _messageBus;
        private readonly List<IService> _services = new List<IService>();
        
        public IHub Deploy(IService service)
        {
            _services.Add(service);
            return this;
        }

        public IHub Undeploy(IService service)
        {
            _services.Remove(service);
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return this;
        }

        public void Start(IMessageBus messageBus)
        {
            if (_state != State.Stopped) return;
            _messageBus = messageBus;
            _messageBus.Publish(new HubStarted());
            _state = State.Started;
        }

        public void Stop()
        {
            if (_state != State.Started) return;
            
            _messageBus.Publish(new HubStopped());
            if (_messageBus is IDisposable disposableHub)
                disposableHub.Dispose();
            
            foreach (var service in _services)
                if (service is IDisposable disposableService)
                    disposableService.Dispose();
            _services.Clear();
            
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