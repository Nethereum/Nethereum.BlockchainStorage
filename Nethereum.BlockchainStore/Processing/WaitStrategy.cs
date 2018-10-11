using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class WaitStrategy : IWaitStrategy
    {
        private static readonly int[] WaitIntervals = {1000, 2000, 5000, 10000, 15000};

        public async Task Apply(int retryCount)
        {
            var intervalMs = retryCount >= WaitIntervals.Length ? 
                WaitIntervals.Last() : 
                WaitIntervals[retryCount];

            await Task.Delay(intervalMs);
        }
    }
}