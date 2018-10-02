using System;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios
{
    public class ContractTransactionProcessorScenario
    {
        protected readonly Mock<IGetTransactionVMStack> _vmStackProxy = new Mock<IGetTransactionVMStack>();
        protected readonly Mock<IVmStackErrorChecker> _vmStackErrorChecker = new Mock<IVmStackErrorChecker>();
        protected readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        protected readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        protected readonly Mock<IAddressTransactionRepository> _addressTransactionRepository = new Mock<IAddressTransactionRepository>();
        protected readonly Mock<ITransactionVMStackRepository> _transactionVmStackRepository = new Mock<ITransactionVMStackRepository>();
        protected readonly Mock<ITransactionLogRepository> _transactionLogRepository = new Mock<ITransactionLogRepository>();
        protected readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        

        protected readonly List<ITransactionLogFilter> _transactionLogFilters = new List<ITransactionLogFilter>();
        protected readonly Transaction _transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
        protected readonly TransactionReceipt _receipt = new TransactionReceipt();
        protected ContractTransactionProcessor _processor;

        protected void InitProcessor(bool enableVmStackProcessing = true)
        {
            _processor = new ContractTransactionProcessor(
                _vmStackProxy.Object, 
                _vmStackErrorChecker.Object,
                _contractRepository.Object, 
                _transactionRepository.Object, 
                _addressTransactionRepository.Object, 
                _transactionVmStackRepository.Object, 
                _transactionLogRepository.Object, 
                _transactionLogFilters);

            _processor.EnabledVmProcessing = enableVmStackProcessing;
        }
    }
}
