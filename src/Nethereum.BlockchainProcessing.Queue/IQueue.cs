using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue
{
    public interface IQueue
    {
        string Name { get; }

        //public CloudQueue CloudQueue { get; }
        Task<int> GetApproxMessageCountAsync();

        Task AddMessageAsync(object content);
    }
}
