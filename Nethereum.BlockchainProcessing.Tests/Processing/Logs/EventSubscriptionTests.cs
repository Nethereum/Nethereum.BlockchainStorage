using Moq;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventSubscriptionTests
    {
        private readonly Mock<IEventMatcher> _matcher = new Mock<IEventMatcher>();
        private readonly Mock<IEventHandlerCoordinator> _handler = new Mock<IEventHandlerCoordinator>();
        private readonly EventSubscription _eventSubscription;
        private readonly EventABI _eventAbi = new EventABI("test");
        private readonly EventSubscriptionStateDto _eventSubscriptionState = new EventSubscriptionStateDto();

        public EventSubscriptionTests()
        {
            _matcher.Setup(m => m.Abi).Returns(_eventAbi);
            _eventSubscription = new EventSubscription(
                id: 1, 
                subscriberId: 2, 
                matcher: _matcher.Object, 
                handler: _handler.Object, 
                state: _eventSubscriptionState);
        }

        [Fact]
        public void HasCorrectId()
        {
            Assert.Equal(1, _eventSubscription.Id);
        }

        [Fact]
        public void HasCorrectSubscriberId()
        {
            Assert.Equal(2, _eventSubscription.SubscriberId);
        }

        [Fact]
        public void MatcherCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventSubscription(1, 1, null, _handler.Object, _eventSubscriptionState));
        }

        [Fact]
        public void HandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventSubscription(1, 1, _matcher.Object, null, _eventSubscriptionState));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InvokesMatcher(bool match)
        {
            var log = new FilterLog();
            _matcher.Setup(m => m.IsMatch(log)).Returns(match);

            Assert.Equal(match, _eventSubscription.IsLogForEvent(log));
        }

        [Fact]
        public async Task InvokesHandler()
        {
            var logs = new FilterLog[]{new FilterLog(), new FilterLog()};
            
            var logsHandled = new List<FilterLog>();

            _handler
                .Setup(m => m.HandleAsync(_eventAbi, logs))
                .Returns<EventABI, FilterLog[]>((abi, l) => 
            {
                logsHandled.AddRange(l);
                return Task.CompletedTask;
            });

            await _eventSubscription.ProcessLogsAsync(logs);

            Assert.Equal(logs, logsHandled);
        }
    }
}
