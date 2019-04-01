using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueue
    {
        string Name {get;}
        Task AddMessageAsync(object content);
    }
}
