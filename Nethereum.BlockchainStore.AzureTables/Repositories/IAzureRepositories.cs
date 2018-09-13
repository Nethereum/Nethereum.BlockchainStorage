using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public interface IAzureTableBlockRepository: IBlockRepository
    {
        Task<Block> GetBlockAsync(HexBigInteger blockNumber);
    }

    public interface IAzureTableContractRepository : IContractRepository
    {
        Task<Contract> FindByAddressAsync(string contractAddress);
        bool IsCached(string contractAddress);
    }

    public interface IAzureTableAddressTransactionRepository : IAddressTransactionRepository
    {
        Task<AddressTransaction> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string transactionHash);
    }

    public interface IAzureTableTransactionRepository : ITransactionRepository
    {
        Task<Transaction> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string transactionHash);
    }

    public interface IAzureTableTransactionLogRepository : ITransactionLogRepository
    {
        Task<TransactionLog> FindByTransactionHashAndLogIndexAsync(string transactionHash, long logIndex);
    }

    public interface IAzureTableTransactionVMStackRepository : ITransactionVMStackRepository
    {
        Task<TransactionVmStack> FindByTransactionHashAync(string transactionHash);
    }

    public interface IBlockchainStoreAzureTablesRepositoryFactory : IBlockchainStoreRepositoryFactory
    {
        IAzureTableAddressTransactionRepository CreateAzureTablesAddressTransactionRepository();
        IAzureTableBlockRepository CreateAzureTablesBlockRepository();
        IAzureTableContractRepository CreateAzureTablesContractRepository();
        IAzureTableTransactionRepository CreateAzureTablesTransactionRepository();
        IAzureTableTransactionLogRepository CreateAzureTablesTransactionLogRepository();
        IAzureTableTransactionVMStackRepository CreateAzureTablesTransactionVmStackRepository();
    }
}
