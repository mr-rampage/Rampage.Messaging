using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Hub
{
    public sealed class ServiceNodeFactory<TService, TMessage> : IServiceNode<TMessage>
    {
        private Unsubscribe _unsubscribe;

        private readonly ReadOnlyDictionary<Type, Action<TMessage>> _handlerByMessageType;

        public ServiceNodeFactory()
        {
            var instance = Activator.CreateInstance<TService>();
            var methodInfo = typeof(TService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var dictionary = methodInfo
                .Where(IsMessageHandler)
                .ToDictionary(GetMessageType, GetMessageHandler(instance));
            _handlerByMessageType = new ReadOnlyDictionary<Type, Action<TMessage>>(dictionary);
        }

        public void Start(IMessageBus<TMessage> messageBus)
        {
            _unsubscribe = messageBus.Subscribe(Combinators.Warbler<TMessage>(SelectHandler));
        }

        private Action<TMessage> SelectHandler(TMessage message) =>
            _handlerByMessageType.ContainsKey(message.GetType())
                ? _handlerByMessageType[message.GetType()]
                : _ => { };

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