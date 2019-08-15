using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue
{
    public interface IQueue
    {
        string Name { get; }

        Task<int> GetApproxMessageCountAsync();

        Task AddMessageAsync(object content);
    }
}
