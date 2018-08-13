using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.EFCore.Repositories
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

                MapValues(contractAddress, code, transaction, contract);

                contract.UpdateRowDates();

                if (contract.IsNew())
                    context.Contracts.Add(contract);
                else
                    context.Contracts.Update(contract);

                await context.SaveChangesAsync().ConfigureAwait(false) ;

                _cachedContracts.AddOrUpdate(contract.Address, contract,
                    (s, existingContract) => contract);
            }
        }

        private void MapValues(string contractAddress, string code, Transaction transaction, Contract contract)
        {
            contract.Address = contractAddress;
            contract.Code = code;
            contract.TransactionHash = transaction.TransactionHash;
            contract.Creator = transaction.From;
        }
    }
}
