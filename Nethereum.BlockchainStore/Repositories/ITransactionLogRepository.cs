using Nethereum.BlockchainStore.Entities;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionLogRepository
    {
        Task UpsertAsync(Log log);
        Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx);
    }
}