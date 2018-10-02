using System;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class TransactionProcessorTests
    {
        private const string TxHash = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9";
        readonly Mock<ITransactionProxy> _mockTransactionProxy = new Mock<ITransactionProxy>();
        readonly Mock<IContractTransactionProcessor> _mockContractTransactionProcessor = new Mock<IContractTransactionProcessor>();
        readonly Mock<IValueTransactionProcessor> _mockValueTransactionProcessor = new Mock<IValueTransactionProcessor>();
        readonly Mock<IContractCreationTransactionProcessor> _mockContractCreationTransactionProcessor = new Mock<IContractCreationTransactionProcessor>();
        readonly List<ITransactionFilter> _transactionFilters = new List<ITransactionFilter>();
        readonly List<ITransactionReceiptFilter> _transactionReceiptFilters = new List<ITransactionReceiptFilter>();

        private readonly BlockWithTransactionHashes _blockWithTransactionHashes = new BlockWithTransactionHashes
        {
            Number = new HexBigInteger(1),
            TransactionHashes = new string[]{TxHash},
            Timestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        private TransactionProcessor CreateTransactionProcessor()
        {
            return new TransactionProcessor(
                _mockTransactionProxy.Object,
                _mockContractTransactionProcessor.Object,
                _mockValueTransactionProcessor.Object,
                _mockContractCreationTransactionProcessor.Object,
                _transactionFilters,
                _transactionReceiptFilters);
        }

        [Fact]
        public async Task EnabledContractCreationProcessing_DefaultsToTrue()
        {
            var txProcessor = CreateTransactionProcessor();
            Assert.True(txProcessor.EnabledContractCreationProcessing);
        }

        [Fact]
        public async Task ProcessTransactionAsync_Processes_ContractCreationTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            var (stubTransaction, stubTransactionReceipt) = CreateContractCreationTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            _mockContractCreationTransactionProcessor.Setup(p =>
                p.ProcessTransactionAsync(stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp))
                .Returns(Task.CompletedTask);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            _mockContractCreationTransactionProcessor.VerifyAll();
            VerifyContractTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
            VerifyValueTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenDisabled_IgnoresContractCreationTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            txProcessor.EnabledContractCreationProcessing = false;
            Assert.False(txProcessor.EnabledContractCreationProcessing);

            var (stubTransaction, stubTransactionReceipt) = CreateContractCreationTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            VerifyNothingWasProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenTxDoesNotMatchFilter_IgnoresTransaction()
        {
            var (stubTransaction, stubTransactionReceipt) = CreateContractCreationTransaction();
            var mockTxFilter = new Mock<ITransactionFilter>();
            mockTxFilter.Setup(f => f.IsMatchAsync(stubTransaction)).ReturnsAsync(false);
            _transactionFilters.Add(mockTxFilter.Object);

            var txProcessor = CreateTransactionProcessor();
            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            VerifyNothingWasProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenTxReceiptDoesNotMatchFilter_IgnoresTransaction()
        {
            var (stubTransaction, stubTransactionReceipt) = CreateContractCreationTransaction();
            var mockTxFilter = new Mock<ITransactionReceiptFilter>();
            mockTxFilter.Setup(f => f.IsMatchAsync(stubTransactionReceipt)).ReturnsAsync(false);
            _transactionReceiptFilters.Add(mockTxFilter.Object);

            var txProcessor = CreateTransactionProcessor();
            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            VerifyNothingWasProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_Processes_ContractTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            Assert.True(txProcessor.EnabledContractProcessing);

            var (stubTransaction, stubTransactionReceipt) = CreateContractTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            _mockContractTransactionProcessor.Setup(p => p.IsTransactionForContractAsync(stubTransaction))
                .ReturnsAsync(true);

            _mockContractTransactionProcessor.Setup(p =>
                    p.ProcessTransactionAsync(stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp))
                .Returns(Task.CompletedTask);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            _mockContractTransactionProcessor.VerifyAll();
            VerifyContractCreationTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
            VerifyValueTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenDisabled_IgnoresContractTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            txProcessor.EnabledContractProcessing = false;
            Assert.False(txProcessor.EnabledContractProcessing);

            var (stubTransaction, stubTransactionReceipt) = CreateContractTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            _mockContractTransactionProcessor.Setup(p => p.IsTransactionForContractAsync(stubTransaction))
                .ReturnsAsync(true);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            VerifyNothingWasProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_Processes_ValueTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            Assert.True(txProcessor.EnabledValueProcessing);

            var (stubTransaction, stubTransactionReceipt) = CreateValueTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            _mockContractTransactionProcessor.Setup(p => p.IsTransactionForContractAsync(stubTransaction))
                .ReturnsAsync(false);

            _mockValueTransactionProcessor.Setup(p =>
                    p.ProcessTransactionAsync(
                        stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp))
                .Returns(Task.CompletedTask);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            _mockValueTransactionProcessor.VerifyAll();
            VerifyContractCreationTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
            VerifyContractTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenDisabled_IgnoresValueTransactions()
        {
            var txProcessor = CreateTransactionProcessor();
            txProcessor.EnabledValueProcessing = false;
            Assert.False(txProcessor.EnabledValueProcessing);

            var (stubTransaction, stubTransactionReceipt) = CreateValueTransaction();

            MockGetTransactionAndReceiptCalls(stubTransaction, stubTransactionReceipt);

            _mockContractTransactionProcessor.Setup(
                    p => p.IsTransactionForContractAsync(stubTransaction))
                .ReturnsAsync(false);

            await txProcessor.ProcessTransactionAsync(TxHash, _blockWithTransactionHashes);

            VerifyNothingWasProcessed(stubTransaction, stubTransactionReceipt);
        }

        private void VerifyNothingWasProcessed
            (Transaction stubTransaction, TransactionReceipt stubTransactionReceipt)
        {
            VerifyContractTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);

            VerifyContractCreationTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);

            VerifyValueTransactionWasNotProcessed(stubTransaction, stubTransactionReceipt);
        }

        private void VerifyValueTransactionWasNotProcessed(
            Transaction stubTransaction, TransactionReceipt stubTransactionReceipt)
        {
            _mockValueTransactionProcessor.Verify(p =>
                    p.ProcessTransactionAsync(
                        stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp),
                Times.Never);
        }

        private void VerifyContractCreationTransactionWasNotProcessed(
            Transaction stubTransaction, TransactionReceipt stubTransactionReceipt)
        {
            _mockContractCreationTransactionProcessor.Verify(p =>
                    p.ProcessTransactionAsync(
                        stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp),
                Times.Never);
        }

        private void VerifyContractTransactionWasNotProcessed(
            Transaction stubTransaction, TransactionReceipt stubTransactionReceipt)
        {
            _mockContractTransactionProcessor.Verify(p =>
                    p.ProcessTransactionAsync(
                        stubTransaction, stubTransactionReceipt, _blockWithTransactionHashes.Timestamp),
                Times.Never);
        }

        private (Transaction tx, TransactionReceipt receipt) CreateValueTransaction()
        {
            var stubTransaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba28"};
            var stubTransactionReceipt = new TransactionReceipt{};
            return (stubTransaction, stubTransactionReceipt);
        }

        private (Transaction tx, TransactionReceipt receipt) CreateContractCreationTransaction()
        {
            var stubTransaction = new Transaction { To = string.Empty};
            var stubTransactionReceipt = new TransactionReceipt
            {
                ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
            };
            return (stubTransaction, stubTransactionReceipt);
        }

        private (Transaction tx, TransactionReceipt receipt) CreateContractTransaction()
        {
            var stubTransaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            var stubTransactionReceipt = new TransactionReceipt
            {
                ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
            };
            return (stubTransaction, stubTransactionReceipt);
        }

        private void MockGetTransactionAndReceiptCalls(Transaction stubTransaction, TransactionReceipt stubTransactionReceipt)
        {
            _mockTransactionProxy.Setup(p => p.GetTransactionByHash(TxHash)).ReturnsAsync(stubTransaction);
            _mockTransactionProxy.Setup(p => p.GetTransactionReceipt(TxHash)).ReturnsAsync(stubTransactionReceipt);
        }
    }
}
