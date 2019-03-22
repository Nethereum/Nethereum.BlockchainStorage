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
    public class LogProcessorTests
    {
        private readonly Mock<IEventMatcher> _matcher = new Mock<IEventMatcher>();
        private readonly Mock<IEventHandler> _handler = new Mock<IEventHandler>();
        private readonly LogProcessor _processor;
        private readonly EventABI _eventAbi = new EventABI("test");

        public LogProcessorTests()
        {
            _matcher.Setup(m => m.Abi).Returns(_eventAbi);
            _processor = new LogProcessor(_matcher.Object, _handler.Object);
        }

        [Fact]
        public void MatcherCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LogProcessor(null, _handler.Object));
        }

        [Fact]
        public void HandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LogProcessor(_matcher.Object, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InvokesMatcher(bool match)
        {
            var log = new FilterLog();
            _matcher.Setup(m => m.IsMatch(log)).Returns(match);

            Assert.Equal(match, _processor.IsLogForEvent(log));
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

            await _processor.ProcessLogsAsync(logs);

            Assert.Equal(logs, logsHandled);
        }
    }
}
