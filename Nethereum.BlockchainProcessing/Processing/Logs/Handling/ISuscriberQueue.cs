using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IQueue
    {
        string Name {get;}
        Task AddMessageAsync(object content);
    }
}
