using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Repositories;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Contract = Nethereum.BlockchainStore.AzureTables.Entities.Contract;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class ContractRepository : AzureTableRepository<Contract>, IContractRepository
    {
        private static readonly ConcurrentDictionary<string, Contract> CachedContracts = new ConcurrentDictionary<string, Contract>();

        public ContractRepository(CloudTable table) : base(table)
        {
        }

        public async Task<Contract> FindAsync(string contractAddress)
        {
            if (CachedContracts.TryGetValue(contractAddress, out Contract contract))
            {
                return contract;
            }

            var operation = TableOperation.Retrieve<Contract>(contractAddress, "");
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Contract;
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            var contract = await FindAsync(contractAddress).ConfigureAwait(false);
            return contract != null;
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction)
        {
            var contract = Contract.CreateContract(contractAddress, code,
                transaction);

            await UpsertAsync(contract).ConfigureAwait(false);

            CachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
        }

        public virtual async Task FillCache()
        {
            await InitContractsCacheAsync().ConfigureAwait(false);
        }

        public async Task InitContractsCacheAsync()
        {
            var contracts = await FindAllAsync().ConfigureAwait(false);
            CachedContracts.Clear();
            foreach (var contract in contracts)
            {
                CachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
            }
        }

        public async Task<List<Contract>> FindAllAsync()
        {
            var tableQuery = new TableQuery<Contract>();
            var contracts = new List<Contract>();

            TableContinuationToken continuationToken = null;
            TableQuerySegment<Contract> queryResult = null;

            do
            {
                queryResult = await Table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken).ConfigureAwait(false);
                contracts.AddRange(queryResult);
                continuationToken = queryResult.ContinuationToken;
            } 
            while (queryResult.ContinuationToken != null);

            return contracts;
        }

        public async Task<IContractView> FindByAddressAsync(string contractAddress)
        {
            return await FindAsync(contractAddress).ConfigureAwait(false);
        }

        public bool IsCached(string contractAddress)
        {
            return CachedContracts.ContainsKey(contractAddress);
        }
    }
}