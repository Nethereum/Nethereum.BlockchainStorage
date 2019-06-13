using Moq;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing
{
    public class TransactionLogProcessorTests
    {
        protected readonly List<ITransactionLogFilter> _filters;
        protected readonly Mock<ITransactionLogHandler> _mockHandler;
        protected readonly TransactionLogProcessor _processor;

        //Mock<ITransactionLogRepository>

        public TransactionLogProcessorTests()
        {
            _filters = new List<ITransactionLogFilter>();
            _mockHandler = new Mock<ITransactionLogHandler>();
            _processor = new TransactionLogProcessor(_filters, _mockHandler.Object);       
        }

        public class ProcessAsync : TransactionLogProcessorTests
        {
            List<TransactionLogWrapper> _logsSentToHandler = new List<TransactionLogWrapper>();

            public ProcessAsync()
            {
                _mockHandler
                    .Setup(h => h.HandleAsync(It.IsAny<TransactionLogWrapper>()))
                    .Callback<TransactionLogWrapper>(l => { _logsSentToHandler.Add(l); })
                    .Returns(Task.CompletedTask);
            }

            [Fact]
            public async Task When_Logs_Are_Null_Will_Not_Throw()
            {
                var receipt = new TransactionReceipt {Logs = null};
                var tx = new Transaction();
                await _processor.ProcessAsync(tx, receipt);
                Assert.Empty(_logsSentToHandler);
            }

            [Fact]
            public async Task Will_Send_All_Logs_To_Handler()
            {
                const string LogAddress1 = "0x1009b29f2094457d3dba62d1953efea58176ba27";
                const string LogAddress2 = "0x2009b29f2094457d3dba62d1953efea58176ba27";

                var tx = new Transaction();
                var receipt = new TransactionReceipt
                {
                    Logs = new JArray(
                        JObject.FromObject(new {address = LogAddress1}),
                        JObject.FromObject(new {address = LogAddress2})
                        )
                };

                await _processor.ProcessAsync(tx, receipt);

                Assert.Equal(2, _logsSentToHandler.Count);
                Assert.Contains(_logsSentToHandler, item => item.Address == LogAddress1);
                Assert.Contains(_logsSentToHandler, item => item.Address == LogAddress2);
            }

            [Fact]
            public async Task Will_Only_Send_Logs_Which_Match_Filter_To_The_Handler()
            {
                const string LogAddressToMatch = "0x1009b29f2094457d3dba62d1953efea58176ba27";
                const string LogAddressToIgnore = "0x2009b29f2094457d3dba62d1953efea58176ba27";

                var filter = new TransactionLogFilter(f => f.Address == LogAddressToMatch);
                _filters.Add(filter);

                var tx = new Transaction();
                var receipt = new TransactionReceipt
                {
                    Logs = new JArray(
                        JObject.FromObject(new {address = LogAddressToMatch}),
                        JObject.FromObject(new {address = LogAddressToIgnore})
                    )
                };

                await _processor.ProcessAsync(tx, receipt);

                Assert.Single(_logsSentToHandler);
                Assert.Contains(_logsSentToHandler, item => item.Address == LogAddressToMatch);
                Assert.DoesNotContain(_logsSentToHandler, item => item.Address == LogAddressToIgnore);
            }
        }
    }
}
