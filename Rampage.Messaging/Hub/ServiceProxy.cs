using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Rampage.Messaging.Utils;

namespace Rampage.Messaging.Hub
{
    public sealed class ServiceProxy<T> : IService
    {
        private Unsubscribe _unsubscribe;

        private readonly ReadOnlyDictionary<Type, Action<IMessage>> _handlerByMessageType;

        public ServiceProxy()
        {
            var instance = Activator.CreateInstance<T>();
            var methodInfo = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var dictionary = methodInfo
                .Where(IsMessageHandler)
                .ToDictionary(GetMessageType, GetMessageHandler(instance));
            _handlerByMessageType = new ReadOnlyDictionary<Type, Action<IMessage>>(dictionary);
        }

        public void Start(IMessageBus messageBus)
        {
            _unsubscribe = messageBus.Subscribe(Combinators.Warbler<IMessage>(SelectHandler));
        }

        private Action<IMessage> SelectHandler(IMessage message) =>
            _handlerByMessageType.ContainsKey(message.GetType())
                ? _handlerByMessageType[message.GetType()]
                : _ => { };

        public void Stop() => _unsubscribe();

        private static bool IsMessageHandler(MethodInfo method) =>
            method.ReturnType == typeof(void) && method.GetParameters().Length == 1 && method.GetParameters()
                .First()
                .ParameterType.GetInterfaces()
                .Contains(typeof(IMessage));
        
        private static Type GetMessageType(MethodInfo method) => method.GetParameters().First().ParameterType;

        private static Func<MethodInfo, Action<IMessage>> GetMessageHandler(T instance) =>
            method => message => method.Invoke(instance, new object[] {message});
    }
}