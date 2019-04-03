using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors
{
    public interface IFilter<in T>
    {
        Task<bool> IsMatchAsync(T item);
    }
}
