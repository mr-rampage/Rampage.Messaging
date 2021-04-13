using System;
using System.Threading.Tasks;

namespace Rampage.Messaging.Utils
{
    internal static class Combinators
    {
        public static Action<Action<T>> Thrush<T>(T x) => f => f(x);
        public static Func<Action<T>, Task> ThrushAsync<T>(T x) => f => Task.Run(() => f(x));
        public static Action<T> Warbler<T>(Func<T, Action<T>> f) => x => f(x)(x);
    }
}