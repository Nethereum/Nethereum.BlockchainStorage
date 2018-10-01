using System;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;
using System.Linq;
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

        private readonly List<ITransactionLogFilter> _transactionLogFilters = new List<ITransactionLogFilter>();

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

            var logAddresses = new string[] {"address1", "address2"};

            receipt.Logs = JArray.FromObject(logAddresses.Select(a => new {address = a}));

            JObject vmStack = new JObject();
            const string Error = "oops";
            
            _vmStackProxy.Setup(p => p.GetTransactionVmStack(transaction.TransactionHash)).ReturnsAsync(vmStack);
            _vmStackErrorChecker.Setup(e => e.GetError(vmStack)).Returns(Error);

            _transactionVmStackRepository
                .Setup(r => r.UpsertAsync(transaction.TransactionHash, transaction.To, vmStack))
                .Returns(Task.CompletedTask);

            const bool hasError = true;
            const bool hasStackTrace = true;

            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, Error))
                .Returns(Task.CompletedTask);

            foreach (var address in new []{transaction.To}.Concat(logAddresses))
            {
                _addressTransactionRepository.Setup(r =>
                        r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, address, Error,
                            hasStackTrace, null))
                    .Returns(Task.CompletedTask);
            }

            List<Tuple<string, long, JObject>> argsPassedToLogRepo = MockTransactionLogRepo();

            var processor = CreateProcessor();

            //execute
            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            //assert
            _transactionVmStackRepository.VerifyAll();
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
            _transactionLogRepository.VerifyAll();

            VerifyLogsHaveBeenSentToRepo(transaction, logAddresses, argsPassedToLogRepo);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenLogDoesNotMatchFilter_ItIsNotUpsertedInTxLogRepo()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27" };
            var receipt = new TransactionReceipt();

            //add a filter which won't match any log
            _transactionLogFilters.Add(new TransactionLogFilter((log) => Task.FromResult(false)));

            var logAddresses = new string[] { "address1", "address2" };

            receipt.Logs = JArray.FromObject(logAddresses.Select(a => new { address = a }));

            JObject vmStack = new JObject();
            const string Error = "oops";

            _vmStackProxy.Setup(p => p.GetTransactionVmStack(transaction.TransactionHash)).ReturnsAsync(vmStack);
            _vmStackErrorChecker.Setup(e => e.GetError(vmStack)).Returns(Error);

            _transactionVmStackRepository
                .Setup(r => r.UpsertAsync(transaction.TransactionHash, transaction.To, vmStack))
                .Returns(Task.CompletedTask);

            const bool hasError = true;
            const bool hasStackTrace = true;

            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, Error))
                .Returns(Task.CompletedTask);

            foreach (var address in new[] { transaction.To }.Concat(logAddresses))
            {
                _addressTransactionRepository.Setup(r =>
                        r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, address, Error,
                            hasStackTrace, null))
                    .Returns(Task.CompletedTask);
            }

            List<Tuple<string, long, JObject>> argsPassedToLogRepo = MockTransactionLogRepo();

            var processor = CreateProcessor();

            //execute
            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            //assert
            Assert.Empty(argsPassedToLogRepo);
        }

        private List<Tuple<string, long, JObject>> MockTransactionLogRepo()
        {
            var argsPassedToLogRepo = new List<Tuple<string, long, JObject>>();

            _transactionLogRepository
                .Setup(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<JObject>()))
                .Callback<string, long, JObject>((txnHash, idx, log) => { argsPassedToLogRepo.Add(new Tuple<string, long, JObject>(txnHash, idx, log)); })
                .Returns(Task.CompletedTask);

            return argsPassedToLogRepo;
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenVmProcessingIsNotEnabled_TheVmStackProxyIsNotCalled()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27" };
            var receipt = new TransactionReceipt();

            var logAddresses = new string[] { "address1", "address2" };

            receipt.Logs = JArray.FromObject(logAddresses.Select(a => new { address = a }));

            const string Error = "";
            const bool hasError = false;
            const bool hasStackTrace = false;

            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, Error))
                .Returns(Task.CompletedTask);

            foreach (var address in new[] { transaction.To }.Concat(logAddresses))
            {
                _addressTransactionRepository.Setup(r =>
                        r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, address, Error,
                            hasStackTrace, null))
                    .Returns(Task.CompletedTask);
            }

            List<Tuple<string, long, JObject>> argsPassedToLogRepo = MockTransactionLogRepo();

            var processor = CreateProcessor();
            processor.EnabledVmProcessing = false;

            //execute
            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            //assert
            _vmStackProxy.Verify(p => p.GetTransactionVmStack(It.IsAny<string>()), Times.Never);
            _vmStackErrorChecker.Verify(e => e.GetError(It.IsAny<JObject>()), Times.Never());
            _transactionVmStackRepository
                .Verify(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JObject>()), Times.Never);

            _transactionVmStackRepository.VerifyAll();
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
            _transactionLogRepository.VerifyAll();

            VerifyLogsHaveBeenSentToRepo(transaction, logAddresses, argsPassedToLogRepo);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenGetVmStackThrows_And_TxGasEqualsReceiptGas_AnErrorIsAssumed()
        {
            var gas = new HexBigInteger(10);
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27", Gas = gas};
            var receipt = new TransactionReceipt{GasUsed = transaction.Gas};

            var logAddresses = new string[] {"address1", "address2"};

            receipt.Logs = JArray.FromObject(logAddresses.Select(a => new {address = a}));

            _vmStackProxy.Setup(p => p.GetTransactionVmStack(transaction.TransactionHash))
                .Throws(new Exception("Fake GetVMStack exception"));

            const string error = "";
            const bool hasError = true;
            const bool hasStackTrace = false;

            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, error))
                .Returns(Task.CompletedTask);

            foreach (var address in new []{transaction.To}.Concat(logAddresses))
            {
                _addressTransactionRepository.Setup(r =>
                        r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, address, error,
                            hasStackTrace, null))
                    .Returns(Task.CompletedTask);
            }

            List<Tuple<string, long, JObject>> argsPassedToLogRepo = MockTransactionLogRepo();

            var processor = CreateProcessor();

            //execute
            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            //assert
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
            _transactionLogRepository.VerifyAll();

            VerifyLogsHaveBeenSentToRepo(transaction, logAddresses, argsPassedToLogRepo);
        }

        [Fact]
        public async Task ProcessTransactionAsync_WhenStackTraceContainsAnError_ItIsTreatedAsAnError()
        {
            var transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
            var receipt = new TransactionReceipt();

            var logAddresses = new string[] {"address1", "address2"};

            receipt.Logs = JArray.FromObject(logAddresses.Select(a => new {address = a}));

            JObject vmStack = new JObject();
            const string EmptyVmStackError = "";
            
            _vmStackProxy.Setup(p => p.GetTransactionVmStack(transaction.TransactionHash)).ReturnsAsync(vmStack);
            _vmStackErrorChecker.Setup(e => e.GetError(vmStack)).Returns(EmptyVmStackError);

            _transactionVmStackRepository
                .Setup(r => r.UpsertAsync(transaction.TransactionHash, transaction.To, vmStack))
                .Returns(Task.CompletedTask);

            const bool hasError = false;
            const bool hasStackTrace = true;

            _transactionRepository
                .Setup(r => r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, hasStackTrace, EmptyVmStackError))
                .Returns(Task.CompletedTask);

            foreach (var address in new []{transaction.To}.Concat(logAddresses))
            {
                _addressTransactionRepository.Setup(r =>
                        r.UpsertAsync(transaction, receipt, hasError, _blockTimestamp, address, EmptyVmStackError,
                            hasStackTrace, null))
                    .Returns(Task.CompletedTask);
            }

            List<Tuple<string, long, JObject>> argsPassedToLogRepo = MockTransactionLogRepo();

            var processor = CreateProcessor();

            //execute
            await processor.ProcessTransactionAsync(transaction, receipt, _blockTimestamp);

            //assert
            _transactionVmStackRepository.VerifyAll();
            _transactionRepository.VerifyAll();
            _addressTransactionRepository.VerifyAll();
            _transactionLogRepository.VerifyAll();

            VerifyLogsHaveBeenSentToRepo(transaction, logAddresses, argsPassedToLogRepo);
        }

        private static void VerifyLogsHaveBeenSentToRepo(Transaction transaction, string[] logAddresses, List<Tuple<string, long, JObject>> argsPassedToLogRepo)
        {
            Assert.Equal(logAddresses.Length, argsPassedToLogRepo.Count);

            for (var logIdx = 0; logIdx < 2; logIdx++)
            {
                var args = argsPassedToLogRepo.FirstOrDefault(a => a.Item2 == logIdx);
                Assert.NotNull(args);
                Assert.Equal(logAddresses[logIdx], args.Item3["address"]);
                Assert.Equal(transaction.TransactionHash, args.Item1);
            }
        }
    }
}
