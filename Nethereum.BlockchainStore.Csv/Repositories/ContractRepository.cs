﻿using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class ContractRepository : CsvRepositoryBase<Contract>, IContractRepository
    {
        private static readonly ConcurrentDictionary<string, Contract> CachedContracts = new ConcurrentDictionary<string, Contract>();

        public ContractRepository(string csvFolderpath) : base(csvFolderpath, "Contracts")
        {
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            var contract = await FindByAddressAsync(contractAddress).ConfigureAwait(false);
            return contract != null;
        }

        public async Task FillCache()
        {
            CachedContracts.Clear();

            await EnumerateAsync(contract =>
            {
                CachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
            });
        }

        public async Task<IContractView> FindByAddressAsync(string contractAddress)
        {
            if (CachedContracts.TryGetValue(contractAddress, out Contract contract))
            {
                return contract;
            }

            return await FindAsync(c => c.Address == contractAddress).ConfigureAwait(false);
        }

        public bool IsCached(string contractAddress)
        {
            return CachedContracts.ContainsKey(contractAddress);
        }

        public async Task UpsertAsync(string contractAddress, string code, RPC.Eth.DTOs.Transaction transaction)
        {
            var contract = new Contract();

            contract.Map(contractAddress, code, transaction);
            contract.UpdateRowDates();

            await Write(contract).ConfigureAwait(false);

            CachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
        }

        protected override ClassMap<Contract> CreateClassMap()
        {
            return ContractMap.Instance;
        }
    }

    public class ContractMap : ClassMap<Contract>
    {
        public static ContractMap Instance = new ContractMap();

        public ContractMap()
        {
            AutoMap();
        }
    }
}
