using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rampage.Messaging.Bus;

namespace Rampage.Messaging.Test
{
    [TestClass]
    public class MessageBusTest
    {
        public static IEnumerable<object[]> Implementations()
        {
            yield return new object[] {new DataFlowMessageBus()};
            yield return new object[] {new EventMessageBus()};
            yield return new object[] {new ParallelMessageBus()};
            yield return new object[] {new PLinqMessageBus()};
            yield return new object[] {new TaskMessageBus()};
        }

        [TestMethod]
        public async Task TestTaskPublish()
        {
            await TestPublish(new TaskMessageBus());
        }

        [DataTestMethod]
        [DynamicData(nameof(Implementations), DynamicDataSourceType.Method)]
        public async Task TestPublish(IMessageBus fixture)
        {
            var handlers =
                new List<(Action<IMessage> handler, TaskCompletionSource<List<IMessage>> promise)>
                {
                    CaptureMessages(3), 
                    CaptureMessages(3),
                    CaptureMessages(3),
                    CaptureMessages(3)
                };

            foreach (var (handler, _) in handlers)
            {
                fixture.Subscribe(handler);
            }

            fixture.Publish(new FakeEventMessage {Value = 1});
            fixture.Publish(new FakeEventMessage {Value = 2});
            fixture.Publish(new FakeEventMessage {Value = 3});

            var actuals = await Task.WhenAll(handlers.Select(pair => pair.promise.Task));
            var expected = new List<IMessage>
            {
                new FakeEventMessage {Value = 1},
                new FakeEventMessage {Value = 2},
                new FakeEventMessage {Value = 3}
            };

            foreach (var actual in actuals)
            {
                CollectionAssert.AreEqual(expected, actual, "Should process all messages in order on all subscribers");
            }

            if (fixture is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(Implementations), DynamicDataSourceType.Method)]
        public void TestUnsubscribe(IMessageBus fixture)
        {
            var (handler, promise) = CaptureMessages(3);
            var unsubscribe = fixture.Subscribe(handler);
            unsubscribe();
            fixture.Publish(new FakeEventMessage {Value = 1});
            fixture.Publish(new FakeEventMessage {Value = 2});
            fixture.Publish(new FakeEventMessage {Value = 3});

            Assert.IsFalse(promise.Task.IsCompleted, "Should not process messages if unsubscribed");
        }

        private static readonly Random Random = new Random(new DateTime().Millisecond);

        private static (Action<IMessage> handler, TaskCompletionSource<List<IMessage>> promise)
            CaptureMessages(uint messageCount)
        {
            var capturedMessages = new List<IMessage>();
            var promise = new TaskCompletionSource<List<IMessage>>();
            return (Handler, promise);

            async void Handler(IMessage message)
            {
                capturedMessages.Add(message);
                await Task.Delay(Random.Next(0, 100));
                if (capturedMessages.Count == messageCount) promise.TrySetResult(capturedMessages);
            }
        }

        private struct FakeEventMessage : IMessage
        {
            public double Value { get; init; }
        }
    }
}