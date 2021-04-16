using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rampage.Messaging.Bus;
using Rampage.Messaging.Hub;

namespace Rampage.Messaging.Test
{
    [TestClass]
    public class HubTest
    {
        [TestMethod]
        public void TestStartAndStop()
        {
            var bus = new ParallelMessageBus<FakeService.IMessage>();
            var fixture = new HubNode<FakeService.IMessage>()
                .Deploy(new FakeServiceNode(new FakeService()))
                .Deploy(new ServiceNodeFactory<FakeService, FakeService.IMessage>());
            fixture.Start(bus);
            bus.Publish(new FakeService.DoWorkA());
            bus.Publish(new FakeService.DoWorkB());
            bus.Publish(new FakeService.DoWorkC());
            fixture.Stop();
        }

        private sealed class FakeServiceNode: IServiceNode<FakeService.IMessage>
        {
            private readonly FakeService _fakeService;
            private Unsubscribe _unsubscribe;

            public FakeServiceNode(FakeService fakeService)
            {
                _fakeService = fakeService;
            }

            public void Start(IMessageBus<FakeService.IMessage> messageBus)
            {
                _unsubscribe = messageBus.Subscribe(message =>
                {
                    switch (message)
                    {
                        case FakeService.DoWorkA workA:
                            _fakeService.HandleWorkA(workA);
                            break;
                        case FakeService.DoWorkB workB:
                            FakeService.HandleWorkB(workB);
                            break;
                    }
                });
            }

            public void Stop()
            {
                _unsubscribe();
            }
        }

        private sealed class FakeService
        {
            public interface IMessage {}
            
            public void HandleWorkA(DoWorkA _workA)
            {
                Trace.WriteLine("A was called");
            }

            public static void HandleWorkB(DoWorkB _workB)
            {
                Trace.WriteLine("B was called");
            }

            public readonly struct DoWorkA : IMessage
            {
            }

            public readonly struct DoWorkB : IMessage
            {
            }

            public readonly struct DoWorkC : IMessage
            {
            }
        }
    }
}