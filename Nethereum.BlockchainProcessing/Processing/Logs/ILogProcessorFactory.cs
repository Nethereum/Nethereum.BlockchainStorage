using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogProcessorFactory
    {
        Task<List<ILogProcessor>> GetLogProcessorsAsync(long partitionId);
    }
}
