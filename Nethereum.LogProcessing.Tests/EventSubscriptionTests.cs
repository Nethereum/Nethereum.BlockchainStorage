using Moq;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class EventSubscriptionTests
    {
        private readonly Mock<IEventMatcher> _matcher = new Mock<IEventMatcher>();
        private readonly Mock<IEventHandlerManager> _handler = new Mock<IEventHandlerManager>();
        private readonly EventSubscription _eventSubscription;
        private readonly EventABI _eventAbi = new EventABI("test");
        private readonly EventABI[] _eventAbis;
        private readonly EventSubscriptionStateDto _eventSubscriptionState = new EventSubscriptionStateDto();

        public EventSubscriptionTests()
        {
            _eventAbis = new[] { _eventAbi };
            _matcher.Setup(m => m.Abis).Returns(_eventAbis);
            _eventSubscription = new EventSubscription(
                id: 1,
                subscriberId: 2,
                matcher: _matcher.Object,
                handlerManager: _handler.Object,
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
            var logs = new FilterLog[] { new FilterLog(), new FilterLog() };

            var logsHandled = new List<FilterLog>();

            _handler
                .Setup(m => m.HandleAsync(_eventSubscription, _eventAbis, logs))
                .Returns<IEventSubscription, EventABI[], FilterLog[]>((subscription, abi, l) =>
            {
                logsHandled.AddRange(l);
                return Task.CompletedTask;
            });

            await _eventSubscription.ProcessLogsAsync(logs);

            Assert.Equal(logs, logsHandled);
        }

        [Fact]
        public void FromEventAbis()
        {
            var eventsToMatch = TestData.Contracts.StandardContract.ContractAbi.Events;
            var eventSubscription = new EventSubscription(eventsToMatch);

            Assert.Equal(eventsToMatch, eventSubscription.Matcher.Abis);

            var transferLog = TestData.Contracts.StandardContract.SampleTransferLog();
            var genericLog = new FilterLog();

            Assert.True(eventSubscription.IsLogForEvent(transferLog));
            Assert.False(eventSubscription.IsLogForEvent(genericLog));
        }

        [Fact]
        public void FromEventAbis_SetsDefaultDependencies()
        {
            var state = new EventSubscriptionStateDto();
            var eventHandlerHistoryDb = new Mock<IEventHandlerHistoryRepository>().Object;
            var contractAddresses = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354" };
            var parameterConditions = new[] { ParameterCondition.Create(1, ParameterConditionOperator.Equals, "x") };

            var eventSubscription = new EventSubscription(
                TestData.Contracts.StandardContract.ContractAbi.Events,
                contractAddresses,
                parameterConditions,
                1, 2, state, eventHandlerHistoryDb);

            Assert.Equal(1, eventSubscription.Id);
            Assert.Equal(2, eventSubscription.SubscriberId);

            var eventHandlerManager = eventSubscription.HandlerManager as EventHandlerManager;
            Assert.NotNull(eventHandlerManager);
            Assert.Same(eventHandlerHistoryDb, eventHandlerManager.History);

            Assert.IsType<EventHandlerManager>(eventSubscription.HandlerManager);
            Assert.Same(state, eventSubscription.State);

            var eventMatcher = eventSubscription.Matcher as EventMatcher;
            Assert.NotNull(eventMatcher);
            var eventAddressMatcher = eventMatcher.AddressMatcher as EventAddressMatcher;
            Assert.NotNull(eventAddressMatcher);
            Assert.Equal(contractAddresses, eventAddressMatcher.AddressesToMatch);

            var parameterMatcher = eventMatcher.ParameterMatcher as EventParameterMatcher;
            Assert.NotNull(eventMatcher.ParameterMatcher);
            Assert.Equal(parameterConditions, parameterMatcher.ParameterConditions);
        }
    }
}
