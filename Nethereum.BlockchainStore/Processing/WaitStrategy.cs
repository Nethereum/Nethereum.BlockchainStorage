using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class WaitStrategy : IWaitStrategy
    {
        private readonly int[] _waitIntervals = {1000, 2000, 5000, 10000, 15000};

        public async Task Apply(int retryCount)
        {
            var intervalMs = retryCount >= _waitIntervals.Length ? 
                _waitIntervals.Last() : 
                _waitIntervals[retryCount];

            await Task.Delay(intervalMs);
        }
    }
}