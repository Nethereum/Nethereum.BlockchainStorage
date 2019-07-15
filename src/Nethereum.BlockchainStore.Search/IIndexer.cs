using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public interface IIndexer
    {
        int Indexed { get; }
        Task<long> DocumentCountAsync();
    }
}
