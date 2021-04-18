using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rampage.Messaging.Hub
{
    public sealed class ServiceNodeFactory<TService, TMessage> : IServiceNode<TMessage>
    {
        private readonly Func<TService> _createInstance;
        private Unsubscribe _unsubscribe;

        public ServiceNodeFactory(Func<TService> createInstance)
        {
            _createInstance = createInstance;
        }

        public void Start(IMessageBus<TMessage> messageBus)
        {
            var instance = _createInstance();
            var handlerByMessageType = typeof(TService)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(IsMessageHandler)
                .ToDictionary(GetMessageType, GetMessageHandler(instance));
            var unsubscribe = messageBus.Subscribe(SelectHandler(handlerByMessageType));
            _unsubscribe = () =>
            {
                unsubscribe();
                instance = default;
            };
        }

        private static Action<TMessage> SelectHandler(IDictionary<Type, Action<TMessage>> handlerByMessageType) =>
            message =>{
                if (handlerByMessageType.ContainsKey(message.GetType()))
                    handlerByMessageType[message.GetType()](message);
            };

        public void Stop() => _unsubscribe();

        private static bool IsMessageHandler(MethodInfo method) =>
            method.ReturnType == typeof(void) && method.GetParameters().Length == 1 && method.GetParameters()
                .First()
                .ParameterType.GetInterfaces()
                .Contains(typeof(TMessage));
        
        private static Type GetMessageType(MethodInfo method) => method.GetParameters().First().ParameterType;

        private static Func<MethodInfo, Action<TMessage>> GetMessageHandler(TService instance) =>
            method => message => method.Invoke(instance, new object[] {message});
    }
}