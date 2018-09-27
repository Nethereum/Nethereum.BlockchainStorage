using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ContractCreationTransactionProcessorTests
    {
        private readonly Mock<IGetCode> _getCodeProxy = new Mock<IGetCode>();
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IAddressTransactionRepository> _addressTransactionRepository = new Mock<IAddressTransactionRepository>();
        private ContractCreationTransactionProcessor _contractCreationTransactionProcessor;
        private readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        [Fact]
        public async Task ProcessTransactionAsync_IgnoresNonContractCreationTransactions()
        {
            var transaction = new Transaction {To = "0x1009b29f2094457d3dba62d1953efea58176ba28"};
            var receipt = new TransactionReceipt();
            _contractCreationTransactionProcessor = CreateProcessor();

            await _contractCreationTransactionProcessor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            _getCodeProxy
                .Verify(p => p.GetCode(receipt.ContractAddress), 
                Times.Never);

            _contractRepository
                .Verify(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), transaction), 
                Times.Never);

            _addressTransactionRepository
                .Verify(r => r.UpsertAsync(transaction, receipt, It.IsAny<bool>(), _blockTimestamp, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), 
                Times.Never);

            _transactionRepository
                .Verify(r => r.UpsertAsync(receipt.ContractAddress, It.IsAny<string>(), transaction, receipt, It.IsAny<bool>(), _blockTimestamp)
               ,Times.Never);
        }

        [Fact]
        public async Task ProcessTransactionAsync_ProcessesValidTransaction()
        {
            var transaction = new Transaction {To = ""};
            var receipt = new TransactionReceipt{ ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            const string Code = "codecodecodecodeetcetc";

            _getCodeProxy.Setup(p => p.GetCode(receipt.ContractAddress)).ReturnsAsync(Code);

            _contractRepository.Setup(r => r.UpsertAsync(receipt.ContractAddress, Code, transaction)).Returns(Task.CompletedTask).Verifiable();
            _transactionRepository.Setup(r => r.UpsertAsync(receipt.ContractAddress, Code, transaction, receipt, false, _blockTimestamp)).Returns(Task.CompletedTask).Verifiable();
            _addressTransactionRepository.Setup(r => r.UpsertAsync(transaction, receipt, false, _blockTimestamp, null, null, false, receipt.ContractAddress)).Returns(Task.CompletedTask).Verifiable();

            _contractCreationTransactionProcessor = CreateProcessor();

            await _contractCreationTransactionProcessor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            _contractRepository.VerifyAll();
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
        }

        [Theory]
        [InlineData("0x")]
        [InlineData(null)]
        public async Task ProcessTransactionAsync_ProcessesFailedContractCreationTransaction_DoesNotUpsertInContractRepository(string code)
        {
            var transaction = new Transaction {To = ""};
            var receipt = new TransactionReceipt{ ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"};

            _getCodeProxy.Setup(p => p.GetCode(receipt.ContractAddress)).ReturnsAsync(code);

            const bool failedContractCreation = true;
            _transactionRepository.Setup(r => r.UpsertAsync(receipt.ContractAddress, code, transaction, receipt, failedContractCreation, _blockTimestamp)).Returns(Task.CompletedTask).Verifiable();
            _addressTransactionRepository.Setup(r => r.UpsertAsync(transaction, receipt, failedContractCreation, _blockTimestamp, null, null, false, receipt.ContractAddress)).Returns(Task.CompletedTask).Verifiable();

            _contractCreationTransactionProcessor = CreateProcessor();

            await _contractCreationTransactionProcessor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            _contractRepository
                .Verify(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), transaction), 
                    Times.Never);

            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
        }

        private ContractCreationTransactionProcessor CreateProcessor()
        {
            return new ContractCreationTransactionProcessor(
                _getCodeProxy.Object, _contractRepository.Object, _transactionRepository.Object, _addressTransactionRepository.Object);
        }
    }
}
