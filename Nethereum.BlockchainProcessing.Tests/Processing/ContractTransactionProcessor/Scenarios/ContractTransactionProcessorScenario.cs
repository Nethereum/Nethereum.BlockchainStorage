using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using Nethereum.BlockchainProcessing.BlockchainProxy;

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
                _transactionVmStackHandler.Object)
            {
                EnabledVmProcessing = enableVmStackProcessing
            };
        }
    }
}
