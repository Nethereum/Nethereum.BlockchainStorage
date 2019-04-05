using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TestContract = Nethereum.BlockchainProcessing.Tests.Processing.Logs.TestData.Contracts.StandardContract;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventHandlerCoordinatorTests
    {
        [Fact]
        public void AssignsCorrectIds()
        {
            List<(long eventHandlerId, string eventKey)> history = new List<(long eventHandlerId, string eventKey)>();
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(history);
            EventHandlerCoordinator coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId:99);

            Assert.Equal(1, coordinator.SubscriberId);
            Assert.Equal(99, coordinator.EventSubscriptionId);
        }

        [Fact]
        public async Task EventAbiCanBeNull()
        {
            var eventsHandled = new List<DecodedEvent>();
            Mock<IEventHandler> mockHandler = CreateMockEventHandler(eventsHandled);

            var handlers = new IEventHandler[]{mockHandler.Object};

            List<(long eventHandlerId, string eventKey)> history = new List<(long eventHandlerId, string eventKey)>();
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(history);
            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId:1, eventSubscriptionId:99, handlers: handlers);

            await coordinator.HandleAsync(null, new RPC.Eth.DTOs.FilterLog{});

            Assert.Single(eventsHandled);
        }

        [Fact]
        public async Task InjectsStateDataToDecodedEvent()
        {
            var eventsHandled = new List<DecodedEvent>();
            Mock<IEventHandler> mockHandler = CreateMockEventHandler(eventsHandled);
            var handlers = new IEventHandler[]{mockHandler.Object};

            var mockEventHandlerHistoryDb = CreateMockHistoryDb();

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId:99, handlers: handlers);

            var logs = new []{new FilterLog{}};
            await coordinator.HandleAsync(null, logs);

            Assert.Single(eventsHandled);
            Assert.Equal(coordinator.SubscriberId, eventsHandled[0].State["SubscriberId"]);
            Assert.Equal(coordinator.EventSubscriptionId, eventsHandled[0].State["EventSubscriptionId"]);
            Assert.Equal(1, eventsHandled[0].State["HandlerInvocations"]);
        }

        [Fact]
        public async Task WritesToHistory()
        {
            var eventsHandled = new List<DecodedEvent>();
            Mock<IEventHandler> mockHandler = CreateMockEventHandler(eventsHandled);
            var handlers = new IEventHandler[] { mockHandler.Object };

            var history = new List<(long eventHandlerId, string eventKey)>();
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(history);

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId: 99, handlers: handlers);

            var logs = new[] { new FilterLog { } };
            await coordinator.HandleAsync(null, logs);

            Assert.Single(eventsHandled);
            Assert.Single(history);
            Assert.Equal(mockHandler.Object.Id, history[0].eventHandlerId);
            Assert.Equal(eventsHandled[0].Key, history[0].eventKey);  
        }

        [Fact]
        public async Task DecodesEventFromAbi()
        {
            var eventsHandled = new List<DecodedEvent>();
            Mock<IEventHandler> mockHandler = CreateMockEventHandler(eventsHandled);
            var handlers = new IEventHandler[] { mockHandler.Object };

            var mockEventHandlerHistoryDb = CreateMockHistoryDb();

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId: 99, handlers: handlers);

            var logs = new[] { TestContract.SampleTransferLog() };
            await coordinator.HandleAsync(TestContract.TransferEventAbi, logs);

            Assert.Single(eventsHandled);
        }

        [Fact]
        public async Task IgnoresEventsWithInvalidNumberOfIndexes()
        {
            var eventsHandled = new List<DecodedEvent>();
            var mockHandler = CreateMockEventHandler(eventsHandled);
            var handlers = new IEventHandler[] { mockHandler.Object };

            var mockEventHandlerHistoryDb = CreateMockHistoryDb();

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId: 99, handlers: handlers);

            var log = TestContract.SampleTransferLog();
            // deliberately make topic count inconsistent with abi
            log.Topics = log.Topics.Take(log.Topics.Length - 1).ToArray();
            var logs = new[] { log };

            var eventAbi = TestContract.TransferEventAbi;

            await coordinator.HandleAsync(eventAbi, logs);

            Assert.Empty(eventsHandled);
        }

        [Fact]
        public async Task ChecksHistoryToPreventDuplication()
        {
            var eventsHandled = new List<DecodedEvent>();
            var mockHandler = CreateMockEventHandler(eventsHandled);
            var handlers = new IEventHandler[] { mockHandler.Object };

            var history = new List<(long eventHandlerId, string eventKey)>();
            var mockEventHandlerHistoryDb = CreateMockHistoryDb(history);

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId: 1, eventSubscriptionId: 99, handlers: handlers);

            var logs = new[] { new FilterLog { } };

            //fake an entry in the history
            history.Add((mockHandler.Object.Id, new DecodedEvent(null, logs[0]).Key));

            await coordinator.HandleAsync(null, logs);

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
            var handlers = new IEventHandler[]{firstHandler.Object, secondHandler.Object};

            var mockEventHandlerHistoryDb = CreateMockHistoryDb();

            var coordinator = new EventHandlerCoordinator(
                mockEventHandlerHistoryDb.Object, subscriberId:1, eventSubscriptionId:99, handlers: handlers);

            var logs = new []{new FilterLog{}};
            await coordinator.HandleAsync(null, logs);

            Assert.Single(handlerOneEvents);
            Assert.Empty(handlerTwoEvents);
        }

        static Mock<IEventHandlerHistoryDb> CreateMockHistoryDb(List<(long eventHandlerId, string eventKey)> history = null)
        {
            history = history ?? new List<(long eventHandlerId, string eventKey)>();

            var mock = new Mock<IEventHandlerHistoryDb>();
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

    }
}
