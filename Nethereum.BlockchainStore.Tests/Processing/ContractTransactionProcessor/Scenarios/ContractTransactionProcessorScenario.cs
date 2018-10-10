using System;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios
{
    public class ContractTransactionProcessorScenario
    {
        protected readonly Mock<IGetTransactionVMStack> _vmStackProxy = new Mock<IGetTransactionVMStack>();
        protected readonly Mock<IVmStackErrorChecker> _vmStackErrorChecker = new Mock<IVmStackErrorChecker>();
        protected readonly Mock<IContractHandler> _contractHandler = new Mock<IContractHandler>();
        protected readonly Mock<ITransactionHandler> _transactionHandler = new Mock<ITransactionHandler>();
        protected readonly Mock<ITransactionVMStackHandler> _transactionVmStackHandler = new Mock<ITransactionVMStackHandler>();
        protected readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        

        protected readonly List<ITransactionLogFilter> _transactionLogFilters = new List<ITransactionLogFilter>();
        protected readonly Transaction _transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27"};
        protected readonly TransactionReceipt _receipt = new TransactionReceipt {Status = new HexBigInteger(1)};
        protected ContractTransactionProcessor _processor;

        protected void InitProcessor(bool enableVmStackProcessing = true)
        {
            _processor = new ContractTransactionProcessor(
                _vmStackProxy.Object, 
                _vmStackErrorChecker.Object,
                _contractHandler.Object, 
                _transactionHandler.Object, 
                _transactionVmStackHandler.Object);

            _processor.EnabledVmProcessing = enableVmStackProcessing;
        }
    }
}
