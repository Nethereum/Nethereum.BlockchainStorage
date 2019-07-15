using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap
{

    public class CloudTableSetup: CloudTableSetupBase, IBlockchainStoreRepositoryFactory
    {
        public CloudTableSetup(string connectionString, string prefix):base(connectionString, prefix){ }

        public CloudTable GetTransactionsVmStackTable()
        {
            return GetPrefixedTable("TransactionsVmStack");
        }

        public CloudTable GetTransactionsLogTable()
        {
            return GetPrefixedTable("TransactionLogs");
        }

        public CloudTable GetTransactionsTable()
        {
            return GetPrefixedTable("Transactions");
        }

        public CloudTable GetAddressTransactionsTable()
        {
            return GetPrefixedTable("AddressTransactions");
        }

        public CloudTable GetBlocksTable()
        {
            return GetPrefixedTable("Blocks");
        }

        public CloudTable GetContractsTable()
        {
            return GetPrefixedTable("Contracts");
        }

        public CloudTable GetCountersTable()
        {
            return GetPrefixedTable("Counters");
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(GetTransactionsLogTable());
        public IBlockRepository CreateBlockRepository() => new BlockRepository(GetBlocksTable(), GetCountersTable());
        public IContractRepository CreateContractRepository() => new ContractRepository(GetContractsTable());
        public ITransactionLogRepository CreateTransactionLogRepository() => new TransactionLogRepository(GetTransactionsLogTable());
        public ITransactionRepository CreateTransactionRepository() => new TransactionRepository(GetTransactionsTable());
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => new TransactionVMStackRepository(GetTransactionsVmStackTable());
        public IBlockProgressRepository CreateBlockProgressRepository() => new BlockProgressRepository(GetCountersTable());

        public async Task DeleteAllTables()
        {
            var options = new TableRequestOptions { };
            var operationContext = new OperationContext() { };
            await GetCountersTable().DeleteIfExistsAsync(options, operationContext);
            await GetContractsTable().DeleteIfExistsAsync(options, operationContext);
            await GetAddressTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetBlocksTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsLogTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsVmStackTable().DeleteIfExistsAsync(options, operationContext);
        }
    }
}