using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rampage.Messaging.Impl;

namespace Rampage.Messaging.Test
{
    [TestClass]
    public class IHubTest
    {
        [TestMethod]
        public void TestStart()
        {
            var fixture = new HubService()
                .Deploy(new FakeService())
                .Deploy(new FakeService());
            fixture.Start(new ParallelMessageBus());
            fixture.Stop();
        }

        private sealed class FakeService: IService
        {
            private Unsubscribe _unsubscribe;

            public void Start(IMessageBus messageBus)
            {
                _unsubscribe = messageBus.Subscribe(Combinators.Warbler<IMessage>(RouteMessage));
            }

            private static Action<IMessage> RouteMessage(IMessage message)
            {
                switch (message)
                {
                    case DoWorkA _ : return HandleWorkA;
                    case DoWorkB _ : return HandleWorkB;
                    case DoWorkC _ : return HandleWorkC;
                }

                return _ => { };
            }

            private static void HandleWorkA(IMessage message)
            {
                
            }
            
            private static void HandleWorkB(IMessage message)
            {
                
            }
            
            private static void HandleWorkC(IMessage message)
            {
                
            }

            public void Stop()
            {
                _unsubscribe();
            }
            
            public readonly struct DoWorkA : IMessage {}
            public readonly struct DoWorkB : IMessage {}
            public readonly struct DoWorkC : IMessage {}
        }
        
    }
}