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
            return GetTable(prefix + "TransactionsVmStack");
        }

        public CloudTable GetTransactionsLogTable()
        {
            return GetTable(prefix + "TransactionsLog");
        }

        public CloudTable GetTransactionsTable()
        {
            return GetTable(prefix + "Transactions");
        }

        public CloudTable GetAddressTransactionsTable()
        {
            return GetTable(prefix + "AddressTransactions");
        }

        public CloudTable GetBlocksTable()
        {
            return GetTable(prefix + "Blocks");
        }

        public CloudTable GetContractsTable()
        {
            return GetTable(prefix + "Contracts");
        }

        public CloudTable GetCountersTable()
        {
            return GetTable(prefix + "Counters");
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