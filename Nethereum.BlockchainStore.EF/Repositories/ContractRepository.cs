﻿using System.Collections.Concurrent;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class ContractRepository : RepositoryBase, IContractRepository
    {
        private readonly ConcurrentDictionary<string, Contract> _cachedContracts = new ConcurrentDictionary<string, Contract>();

        public ContractRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public bool IsCached(string contractAddress)
        {
            return _cachedContracts.TryGetValue(contractAddress, out Contract val);
        }

        public async Task FillCache()
        {
            _cachedContracts.Clear();
            using (var context = _contextFactory.CreateContext())
            {
                var contracts = await context.Contracts.ToListAsync();
                foreach (var contract in contracts)
                {
                    _cachedContracts.AddOrUpdate(contract.Address, contract, (s, contract1) => contract);
                }
            }
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            if(IsCached(contractAddress))
                return true;

            using (var context = _contextFactory.CreateContext())
            {
                var contract = await context.Contracts.FindByContractAddressAsync(contractAddress).ConfigureAwait(false) ;
                return contract != null;
            }
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var contract = await context.Contracts.FindByContractAddressAsync(contractAddress).ConfigureAwait(false)  ?? new Contract();

                contract.Map(contractAddress, code, transaction);
                contract.UpdateRowDates();

                context.Contracts.AddOrUpdate(contract);

                await context.SaveChangesAsync().ConfigureAwait(false) ;

                _cachedContracts.AddOrUpdate(contract.Address, contract,
                    (s, existingContract) => contract);
            }
        }

    }
}
