using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Queue
{
    public interface IQueue
    {
        string Name { get; }

        Task<int> GetApproxMessageCountAsync();

        Task AddMessageAsync(object content);
    }
}
