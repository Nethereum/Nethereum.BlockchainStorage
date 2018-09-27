using System;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ContractTransactionProcessorTests
    {
        private readonly Mock<IGetTransactionVMStack> _vmStackProxy = new Mock<IGetTransactionVMStack>();
        private readonly Mock<IVmStackErrorChecker> _vmStackErrorChecker = new Mock<IVmStackErrorChecker>();
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IAddressTransactionRepository> _addressTransactionRepository = new Mock<IAddressTransactionRepository>();
        private readonly Mock<ITransactionVMStackRepository> _transactionVmStackRepository = new Mock<ITransactionVMStackRepository>();
        private readonly Mock<ITransactionLogRepository> _transactionLogRepository = new Mock<ITransactionLogRepository>();
        private readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        private readonly IEnumerable<ITransactionLogFilter> _transactionLogFilters = new List<ITransactionLogFilter>();

        private ContractTransactionProcessor CreateProcessor()
        {
            return new ContractTransactionProcessor(
                _vmStackProxy.Object, 
                _vmStackErrorChecker.Object,
                _contractRepository.Object, 
                _transactionRepository.Object, 
                _addressTransactionRepository.Object, 
                _transactionVmStackRepository.Object, 
                _transactionLogRepository.Object, 
                _transactionLogFilters);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("0x")]
        public async Task IsTransactionForContractAsync_WhenToAddressEmpty_ReturnsFalse(string toAddress)
        {
            var transaction = new Transaction { To = toAddress};
            var processor = CreateProcessor();
            Assert.False(await processor.IsTransactionForContractAsync(transaction));
        }

        [Fact]
        public async Task IsTransactionForContractAsync_WhenToAddressIsNotInContractRepo_ReturnsFalse()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            var processor = CreateProcessor();

            _contractRepository.Setup(r => r.ExistsAsync(transaction.To)).ReturnsAsync(false);

            Assert.False(await processor.IsTransactionForContractAsync(transaction));
        }

        [Fact]
        public async Task IsTransactionForContractAsync_WhenToAddressIsInContractRepo_ReturnsTrue()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            var processor = CreateProcessor();

            _contractRepository.Setup(r => r.ExistsAsync(transaction.To)).ReturnsAsync(true);

            Assert.True(await processor.IsTransactionForContractAsync(transaction));
        }

        [Fact]
        public async Task ProcessTransactionAsync()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            var receipt = new TransactionReceipt();

            receipt.Logs = JArray.Parse("[{'address':'address1'},{'address':'address2'}]");

            const string Error = "oops";
            JObject vmStack = new JObject();

            _vmStackProxy.Setup(p => p.GetTransactionVmStack(transaction.TransactionHash)).ReturnsAsync(vmStack);
            _vmStackErrorChecker.Setup(e => e.GetError(vmStack)).Returns(Error);

            _transactionVmStackRepository
                .Setup(r => r.UpsertAsync(transaction.TransactionHash, transaction.To, vmStack))
                .Returns(Task.CompletedTask);

            var hasError = true;
            var hasStackTrace = true;
            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, Error))
                .Returns(Task.CompletedTask);

            _addressTransactionRepository.Setup(r =>
                r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, transaction.To, Error, hasStackTrace, transaction.To))
                .Returns(Task.CompletedTask);

            var processor = CreateProcessor();

            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);


            _transactionVmStackRepository.VerifyAll();
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
        }
    }
}
