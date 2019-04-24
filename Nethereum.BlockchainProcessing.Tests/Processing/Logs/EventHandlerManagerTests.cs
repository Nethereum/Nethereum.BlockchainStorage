using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TestContract = Nethereum.BlockchainProcessing.Tests.Processing.Logs.TestData.Contracts.StandardContract;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventHandlerManagerTests
    {
        [Fact]
        public async Task EventAbiCanBeNull()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);
            var mockEventHandlerHistoryDb = CreateMockHistoryDb();
            var coordinator = new EventHandlerManager(mockEventHandlerHistoryDb.Object);

            await coordinator.HandleAsync(subscription, null, new FilterLog{});

            Assert.Single(eventsHandled);
        }

        [Fact]
        public async Task InjectsStateDataToDecodedEvent()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);
            var mockEventHandlerHistoryDb = CreateMockHistoryDb();
            var coordinator = new EventHandlerManager(mockEventHandlerHistoryDb.Object);

            var logs = new []{new FilterLog{}};
            await coordinator.HandleAsync(subscription, null, logs);

            Assert.Single(eventsHandled);
            Assert.Equal(subscription.SubscriberId, eventsHandled[0].State["SubscriberId"]);
            Assert.Equal(subscription.Id, eventsHandled[0].State["EventSubscriptionId"]);
            Assert.Equal(1, eventsHandled[0].State["HandlerInvocations"]);
            Assert.Equal(1, subscription.State.GetInt("EventsHandled"));
        }

        [Fact]
        public async Task WritesToHistory()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);

            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> history);

            var coordinator = new EventHandlerManager(
                mockEventHandlerHistoryDb.Object);

            var logs = new[] { new FilterLog { } };
            await coordinator.HandleAsync(subscription, null, logs);

            Assert.Single(eventsHandled);
            Assert.Single(history);
            Assert.Equal(subscription.EventHandlers.First().Id, history[0].eventHandlerId);
            Assert.Equal(eventsHandled[0].Key, history[0].eventKey);  
        }

        [Fact]
        public async Task DecodesEventFromAbi()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out _);

            var coordinator = new EventHandlerManager(
                mockEventHandlerHistoryDb.Object);

            var logs = new[] { TestContract.SampleTransferLog() };
            await coordinator.HandleAsync(subscription, new[]{ TestContract.TransferEventAbi }, logs);

            Assert.Single(eventsHandled);
        }

        [Fact]
        public async Task WhenAbisAreNotConfigured_DecodesToGenericEventLog()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out _);

            var coordinator = new EventHandlerManager(
                mockEventHandlerHistoryDb.Object);

            var logs = new[] { new FilterLog() };
            await coordinator.HandleAsync(subscription, new ABI.Model.EventABI[0], logs);

            Assert.Single(eventsHandled);
            Assert.Empty(eventsHandled[0].Event);
        }

        [Fact]
        public async Task WithMultipleAbis_DecodesToFirstMatchingAbi()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out _);

            var coordinator = new EventHandlerManager(
                mockEventHandlerHistoryDb.Object);

            var logs = new[] { TestContract.SampleTransferLog() };

            var abis = new []{ 
                TestContract.ApprovalEventAbi,
                TestContract.TransferEventAbi };

            await coordinator.HandleAsync(subscription, abis, logs);

            Assert.Single(eventsHandled);
            Assert.Equal(
                TestContract.TransferEventAbi.InputParameters.Length, 
                eventsHandled[0].Event.Count);
        }

        [Fact]
        public async Task IgnoresEventsWithInvalidNumberOfIndexes()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);

            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> history);

            var coordinator = new EventHandlerManager(
                mockEventHandlerHistoryDb.Object);

            var log = TestContract.SampleTransferLog();
            // deliberately make topic count inconsistent with abi
            log.Topics = log.Topics.Take(log.Topics.Length - 1).ToArray();
            var logs = new[] { log };

            var eventAbi = TestContract.TransferEventAbi;

            await coordinator.HandleAsync(subscription, new[]{ eventAbi }, logs);

            Assert.Empty(eventsHandled);
        }

        [Fact]
        public async Task ChecksHistoryToPreventDuplication()
        {
            var subscription = CreateMockSubscription(out List<DecodedEvent> eventsHandled);

            var mockEventHandlerHistoryDb = CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> history);

            var coordinator = new EventHandlerManager(mockEventHandlerHistoryDb.Object);

            var logs = new[] { new FilterLog { } };

            //fake an entry in the history
            history.Add((subscription.EventHandlers.First().Id, new DecodedEvent(null, logs[0]).Key));

            await coordinator.HandleAsync(subscription, null, logs);

            Assert.Empty(eventsHandled);
            Assert.Single(history);
        }

        [Fact]
        public async Task WhenOneHandlerReturnsFalseDoesNotInvokeNext()
        {
            var handlerOneEvents = new List<DecodedEvent>();
            var handlerTwoEvents = new List<DecodedEvent>();
            var firstHandler = CreateMockEventHandler(handlerOneEvents, handleSuccessfully: false);
            var secondHandler = CreateMockEventHandler(handlerTwoEvents);

            var subscription = CreateMockSubscription(new []{firstHandler.Object, secondHandler.Object});

            var mockEventHandlerHistoryDb = CreateMockHistoryDb();

            var coordinator = new EventHandlerManager(mockEventHandlerHistoryDb.Object);

            var logs = new []{new FilterLog{}};
            await coordinator.HandleAsync(subscription, null, logs);

            Assert.Single(handlerOneEvents);
            Assert.Empty(handlerTwoEvents);
        }


        private static Mock<IEventHandlerHistoryRepository> CreateMockHistoryDb(List<(long eventHandlerId, string eventKey)> history = null)
        {
            return CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> h);
        }

        private static Mock<IEventHandlerHistoryRepository> CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> history)
        {
            history = new List<(long eventHandlerId, string eventKey)>();
            return CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> h, history);
        }

        private static Mock<IEventHandlerHistoryRepository> CreateMockHistoryDb(out List<(long eventHandlerId, string eventKey)> historyList, List<(long eventHandlerId, string eventKey)> history = null)
        {
            history = history ?? new List<(long eventHandlerId, string eventKey)>();
            historyList = history;

            var mock = new Mock<IEventHandlerHistoryRepository>();
            mock
                .Setup(m => m.AddEventHandlerHistory(It.IsAny<long>(), It.IsAny<string>()))
                .Returns<long, string>((handlerId, eventKey) =>
                {
                    history.Add((handlerId, eventKey));
                    return Task.CompletedTask;
                });
            mock
                .Setup(m => m.ContainsEventHandlerHistory(It.IsAny<long>(), It.IsAny<string>()))
                .Returns<long, string>((handlerId, eventKey) =>
                {
                    var exists = history.Any(h => h.eventHandlerId == handlerId && h.eventKey == eventKey);
                    return Task.FromResult(exists);
                });
            return mock;
        }

        private static Mock<IEventHandler> CreateMockEventHandler(List<DecodedEvent> eventsHandled, bool handleSuccessfully = true)
        {
            var mockHandler = new Mock<IEventHandler>();

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<DecodedEvent>())).Returns<DecodedEvent>((e) =>
            {
                eventsHandled.Add(e);
                return Task.FromResult(handleSuccessfully);
            });

            mockHandler.Setup(h => h.Id).Returns(101);

            return mockHandler;
        }

        private static IEventSubscription CreateMockSubscription(out List<DecodedEvent> eventsHandled, IEnumerable<IEventHandler> handlers = null)
        {
            Mock<IEventSubscription> mockSubscription = CreateMockSubscription();

            eventsHandled = new List<DecodedEvent>();
            handlers = handlers ?? new IEventHandler[] { CreateMockEventHandler(eventsHandled).Object };
            mockSubscription.Setup(s => s.EventHandlers).Returns(handlers);
            return mockSubscription.Object;
        }

        private static IEventSubscription CreateMockSubscription(IEnumerable<IEventHandler> handlers = null)
        {
            var mockSubscription = CreateMockSubscription();
            mockSubscription.Setup(s => s.EventHandlers).Returns(handlers);
            return mockSubscription.Object;
        }

        private static Mock<IEventSubscription> CreateMockSubscription()
        {
            var mockSubscription = new Mock<IEventSubscription>();
            var state = new EventSubscriptionStateDto();
            mockSubscription.Setup(s => s.State).Returns(state);
            return mockSubscription;
        }
    }
}
