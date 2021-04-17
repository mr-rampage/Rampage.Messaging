using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rampage.Messaging.Hub
{
    public sealed class ServiceNodeFactory<TService, TMessage> : IServiceNode<TMessage>
    {
        private Unsubscribe _unsubscribe;

        public void Start(IMessageBus<TMessage> messageBus)
        {
            var instance = CreateInstance(messageBus.Publish);
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

        private static TService CreateInstance(Action<TMessage> dispatcher)
        {
            var type = typeof(TService);
            var constructor = type.GetConstructor(new[] {typeof(Action<TMessage>)});
            return constructor != null
                ? (TService) constructor.Invoke(new object[] {dispatcher})
                : Activator.CreateInstance<TService>();
        }

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