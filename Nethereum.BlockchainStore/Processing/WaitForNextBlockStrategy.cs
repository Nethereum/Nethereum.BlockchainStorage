using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class WaitForNextBlockStrategy
    {
        private int[] waitIntervals = {1000, 2000, 5000, 10000, 15000};

        public async Task Apply(int retryCount)
        {
            var intervalMs = retryCount >= waitIntervals.Length ? waitIntervals.Last() : waitIntervals[retryCount];
            await Task.Delay(intervalMs);
        }
    }
}