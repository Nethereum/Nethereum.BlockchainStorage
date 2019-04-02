using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventHandlerCoordinatorTests
    {
        [Fact]
        public void AssignsCorrectIds()
        {
            EventHandlerCoordinator coordinator = new EventHandlerCoordinator(subscriberId:1, eventSubscriptionId:99);
            Assert.Equal(1, coordinator.SubscriberId);
            Assert.Equal(99, coordinator.EventSubscriptionId);
        }

        [Fact]
        public async Task EventAbiCanBeNull()
        {
            var eventsHandled = new List<DecodedEvent>();
            var mockHandler = new Mock<IEventHandler>();

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<DecodedEvent>())).Returns<DecodedEvent>((e) => 
            {
                eventsHandled.Add(e);
                return Task.FromResult(true);
            });

            var handlers = new IEventHandler[]{mockHandler.Object};

            EventHandlerCoordinator coordinator = new EventHandlerCoordinator(subscriberId:1, eventSubscriptionId:99, handlers: handlers);
            await coordinator.HandleAsync(abi: null, new RPC.Eth.DTOs.FilterLog{});

            Assert.Single(eventsHandled);
        }

        [Fact]
        public async Task InjectsStateDataToDecodedEvent()
        {
            var eventsHandled = new List<DecodedEvent>();
            var mockHandler = new Mock<IEventHandler>();

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<DecodedEvent>())).Returns<DecodedEvent>((e) => 
            {
                eventsHandled.Add(e);
                return Task.FromResult(true);
            });

            var handlers = new IEventHandler[]{mockHandler.Object};

            EventHandlerCoordinator coordinator = new EventHandlerCoordinator(subscriberId:1, eventSubscriptionId:99, handlers: handlers);

            var logs = new []{new RPC.Eth.DTOs.FilterLog{}};
            await coordinator.HandleAsync(abi: null, logs);

            Assert.Single(eventsHandled);
            Assert.Equal(coordinator.SubscriberId, eventsHandled[0].State["SubscriberId"]);
            Assert.Equal(coordinator.EventSubscriptionId, eventsHandled[0].State["EventSubscriptionId"]);
            Assert.Equal(1, eventsHandled[0].State["HandlerInvocations"]);
        }

        [Fact]
        public async Task WhenOneHandlerReturnsFalseDoesNotInvokeNext()
        {
            var handlerOneEvents = new List<DecodedEvent>();
            var handlerTwoEvents = new List<DecodedEvent>();
            var firstHandler = new Mock<IEventHandler>();
            var secondHandler = new Mock<IEventHandler>();

            firstHandler.Setup(h => h.HandleAsync(It.IsAny<DecodedEvent>())).Returns<DecodedEvent>((e) => 
            {
                handlerOneEvents.Add(e);
                return Task.FromResult(false);
            });

            secondHandler.Setup(h => h.HandleAsync(It.IsAny<DecodedEvent>())).Returns<DecodedEvent>((e) => 
            {
                handlerTwoEvents.Add(e);
                return Task.FromResult(true);
            });

            var handlers = new IEventHandler[]{firstHandler.Object, secondHandler.Object};

            EventHandlerCoordinator coordinator = new EventHandlerCoordinator(subscriberId:1, eventSubscriptionId:99, handlers: handlers);

            var logs = new []{new RPC.Eth.DTOs.FilterLog{}};
            await coordinator.HandleAsync(abi: null, logs);

            Assert.Single(handlerOneEvents);
            Assert.Empty(handlerTwoEvents);
        }

        
    }
}
