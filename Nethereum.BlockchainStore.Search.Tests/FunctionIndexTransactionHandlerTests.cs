using Moq;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Xunit;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class FunctionIndexTransactionHandlerTests
    {
        public class TestFunctionMessageDto : FunctionMessage
        {

        }

        [Fact]
        public async Task SendsToIndexInBatches()
        {
            var indexer = new Mock<IFunctionIndexer<TestFunctionMessageDto>>();
            var handler = new FunctionIndexTransactionHandler<TestFunctionMessageDto>(indexer.Object, logsPerIndexBatch: 2);

            var transaction1 = new Mock<TransactionWithReceipt>();
            var transaction2 = new Mock<TransactionWithReceipt>();

            var dto1 = new TestFunctionMessageDto();
            var dto2 = new TestFunctionMessageDto();

            transaction1.Setup(t => t.IsForFunction<TestFunctionMessageDto>()).Returns(true);
            transaction2.Setup(t => t.IsForFunction<TestFunctionMessageDto>()).Returns(true);
            transaction1.Setup(t => t.Decode<TestFunctionMessageDto>()).Returns(dto1);
            transaction2.Setup(t => t.Decode<TestFunctionMessageDto>()).Returns(dto2);

            var indexedFunctionCalls = CaptureCallsToIndexer(indexer);

            await handler.HandleTransactionAsync(transaction1.Object);

            Assert.Empty(indexedFunctionCalls);
            Assert.Equal(1, handler.Pending);

            await handler.HandleTransactionAsync(transaction2.Object);
            Assert.Equal(2, indexedFunctionCalls.Count);
            Assert.Equal(0, handler.Pending);
        }

        [Fact]
        public async Task WhenDisposeIsCalledWillAttemptToIndexPendingItems()
        {
            var indexer = new Mock<IFunctionIndexer<TestFunctionMessageDto>>();
            var indexedFunctionCalls = CaptureCallsToIndexer(indexer);

            using (var handler =
                new FunctionIndexTransactionHandler<TestFunctionMessageDto>(indexer.Object, logsPerIndexBatch: 2))
            {

                var transaction1 = new Mock<TransactionWithReceipt>();
                var dto1 = new TestFunctionMessageDto();

                transaction1.Setup(t => t.IsForFunction<TestFunctionMessageDto>()).Returns(true);
                transaction1.Setup(t => t.Decode<TestFunctionMessageDto>()).Returns(dto1);

                await handler.HandleTransactionAsync(transaction1.Object);
                Assert.Equal(1, handler.Pending);
            }

            //calling dispose should cause app to process remaining pending items
            Assert.Single(indexedFunctionCalls);
        }

        [Fact]
        public async Task WillIgnoreLogsThatDoNotMatchTheEvent()
        {
            var indexer = new Mock<IFunctionIndexer<TestFunctionMessageDto>>();
            var handler = new FunctionIndexTransactionHandler<TestFunctionMessageDto>(indexer.Object, logsPerIndexBatch: 1);

            var transaction1 = new Mock<TransactionWithReceipt>();

            transaction1.Setup(t => t.IsForFunction<TestFunctionMessageDto>()).Returns(false);

            var indexedFunctionCalls = CaptureCallsToIndexer(indexer);

            await handler.HandleTransactionAsync(transaction1.Object);
            Assert.Empty(indexedFunctionCalls);
            Assert.Equal(0, handler.Pending);
            
        }

        private List<(Transaction, TestFunctionMessageDto)> CaptureCallsToIndexer(Mock<IFunctionIndexer<TestFunctionMessageDto>> indexer)
        {
            List<(Transaction, TestFunctionMessageDto)> indexedFunctionCalls = new EditableList<(Transaction, TestFunctionMessageDto)>();

            indexer
                .Setup(i => i.IndexAsync(It.IsAny<IEnumerable<(Transaction, TestFunctionMessageDto)>>()))
                .Returns<IEnumerable<(Transaction, TestFunctionMessageDto)>>(l =>
                {
                    indexedFunctionCalls.AddRange(l);
                    return Task.CompletedTask;
                });

            return indexedFunctionCalls;
        }
    }
}
