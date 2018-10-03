using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ContractCreationTransactionProcessorTests
    {
        private readonly Mock<IGetCode> _getCodeProxy = new Mock<IGetCode>();
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IAddressTransactionRepository> _addressTransactionRepository = new Mock<IAddressTransactionRepository>();
        private ContractCreationTransactionProcessor _processor;
        private readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        [Fact]
        public async Task ProcessTransactionAsync_IgnoresNonContractCreationTransactions()
        {
            var transaction = new Transaction
            {
                To = "0x1009b29f2094457d3dba62d1953efea58176ba28"
            };
            var receipt = new TransactionReceipt();

            _processor = CreateProcessor();

            await _processor.ProcessTransactionAsync(
                transaction, receipt, _blockTimestamp);

            VerifyNothingWasProcessed(transaction, receipt);
        }  

        [Fact]
        public async Task ProcessTransactionAsync_ProcessesValidTransaction()
        {
            var transaction = new Transaction { To = "" };
            var receipt = new TransactionReceipt
            {
                ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
            };
            const string Code = "codecodecodecodeetcetc";

            MockGetCode(receipt, Code);
            MockContractUpsert(transaction, receipt, Code);
            MockTransactionUpsert(transaction, receipt, Code);
            MockAddressTransactionUpsert(transaction, receipt);

            _processor = CreateProcessor();

            await _processor.ProcessTransactionAsync(
                transaction, receipt, _blockTimestamp);

            VerifyContractWasUpserted();
            VerifyTransactionWasUpserted();
        }

        [Theory]
        [InlineData("0x")]
        [InlineData(null)]
        public async Task ProcessTransactionAsync_ProcessesFailedContractCreationTransaction_DoesNotUpsertInContractRepository
            (string code)
        {
            var transaction = new Transaction { To = "" };
            var receipt = new TransactionReceipt
            {
                ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
            };

            MockGetCode(receipt, code);

            const bool failedContractCreation = true;

            MockTransactionUpsert(transaction, receipt, code, failedContractCreation);
            MockAddressTransactionUpsert(transaction, receipt, failedContractCreation);

            _processor = CreateProcessor();

            await _processor.ProcessTransactionAsync(
                transaction, receipt, _blockTimestamp);

            VerifyContractWasNotUpserted(transaction);

            VerifyTransactionWasUpserted();
        }

        private void VerifyTransactionWasUpserted()
        {
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
        }

        private void VerifyContractWasNotUpserted(Transaction transaction)
        {
            _contractRepository
                .Verify(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), transaction),
                    Times.Never);
        }

        private ContractCreationTransactionProcessor CreateProcessor()
        {
            return new ContractCreationTransactionProcessor(
                _getCodeProxy.Object, _contractRepository.Object, _transactionRepository.Object, _addressTransactionRepository.Object);
        }

        private void VerifyNothingWasProcessed(Transaction transaction, TransactionReceipt receipt)
        {
            _getCodeProxy
                .Verify(p => p.GetCode(It.IsAny<string>()),
                    Times.Never);

            _contractRepository
                .Verify(r => r.UpsertAsync(
                        It.IsAny<string>(), It.IsAny<string>(), transaction),
                    Times.Never);

            _addressTransactionRepository
                .Verify(r => r.UpsertAsync(
                        transaction, receipt, It.IsAny<bool>(), _blockTimestamp,
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                        It.IsAny<string>()),
                    Times.Never);

            _transactionRepository
                .Verify(r => r.UpsertAsync(receipt.ContractAddress, It.IsAny<string>(),
                        transaction, receipt, It.IsAny<bool>(), _blockTimestamp)
                    , Times.Never);
        }

        private void MockAddressTransactionUpsert(
            Transaction transaction, TransactionReceipt receipt, bool failedContractCreation = false)
        {
            _addressTransactionRepository
                .Setup(r => r.UpsertAsync(
                    transaction, receipt, failedContractCreation, _blockTimestamp,
                    null, null, false, receipt.ContractAddress))
                .Returns(Task.CompletedTask).Verifiable();
        }

        private void MockTransactionUpsert(Transaction transaction, TransactionReceipt receipt, string code, bool failedContractCreation = false)
        {
            _transactionRepository
                .Setup(r => r.UpsertAsync(
                    receipt.ContractAddress, code, transaction, receipt, failedContractCreation, _blockTimestamp))
                .Returns(Task.CompletedTask).Verifiable();
        }

        private void MockContractUpsert(Transaction transaction, TransactionReceipt receipt, string code)
        {
            _contractRepository
                .Setup(r => r.UpsertAsync(receipt.ContractAddress, code, transaction))
                .Returns(Task.CompletedTask).Verifiable();
        }

        private void MockGetCode(TransactionReceipt receipt, string code)
        {
            _getCodeProxy
                .Setup(p => p.GetCode(receipt.ContractAddress))
                .ReturnsAsync(code);
        }

        private void VerifyContractWasUpserted()
        {
            _contractRepository.VerifyAll();
        }
    }
}
