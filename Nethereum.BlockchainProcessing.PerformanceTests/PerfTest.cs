using Microsoft.Extensions.Logging;
using Nethereum.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{
    public abstract class PerfTest
    {
        protected static readonly ILogger _log = ApplicationLogging.CreateLogger<PerfTest>();

        public virtual Task ConfigureAsync() => Task.CompletedTask;
        protected abstract Task RunAsync();

        protected Stopwatch stopWatch;

        public virtual async Task RunTestAsync()
        {
            await ConfigureAsync();
            stopWatch = Stopwatch.StartNew();
            await RunAsync();
            var elapsed = stopWatch.Elapsed;
            stopWatch.Stop();

            _log.LogInformation("** Finished");
            _log.LogInformation($"** Elapsed: Hours: {elapsed.Hours}, Minutes: {elapsed.Minutes}, Seconds: {elapsed.Seconds}, Ms: {elapsed.Milliseconds}");
        }
    }
}
