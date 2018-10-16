﻿using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessingStrategy: IDisposable
    {
        FilterContainer Filters { get; }
        IBlockHandler BlockHandler { get; }
        ITransactionHandler TransactionHandler { get; }
        ITransactionLogHandler TransactionLogHandler { get; }
        ITransactionVMStackHandler TransactionVmStackHandler { get; }
        IContractHandler ContractHandler { get; }
        Task<long> GetLastBlockProcessedAsync();
        Task FillContractCacheAsync();
        Task WaitForNextBlock(int retryNumber);
        Task PauseFollowingAnError(int retryNumber);
        int MaxRetries { get; }
        long MinimumBlockNumber { get; }
        int MinimumBlockConfirmations { get; }
    }
}
