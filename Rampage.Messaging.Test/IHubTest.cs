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
            var bus = new ParallelMessageBus();
            var fixture = new HubService()
                .Deploy(new ServiceProxy<FakeService>())
                .Deploy(new ServiceProxy<FakeService>());
            fixture.Start(bus);
            bus.Publish(new FakeService.DoWorkA());
            bus.Publish(new FakeService.DoWorkB());
            bus.Publish(new FakeService.DoWorkC());
            fixture.Stop();
        }

        private sealed class FakeService
        {
            public void HandleWorkA(DoWorkA workA)
            {
                Trace.WriteLine("A was called");
            }

            public void HandleWorkB(DoWorkB workB)
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