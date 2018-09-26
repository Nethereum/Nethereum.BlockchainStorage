using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IFilter<in T>
    {
        Task<bool> IsMatchAsync(T item);
    }
}
