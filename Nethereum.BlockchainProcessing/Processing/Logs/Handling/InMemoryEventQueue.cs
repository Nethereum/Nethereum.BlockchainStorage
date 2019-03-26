using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class InMemoryEventQueue : IDecodedEventHandler
    {
        Queue<DecodedEvent> _queue = new Queue<DecodedEvent>();

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            _queue.Enqueue(decodedEvent);

            Debug.Write(JsonConvert.SerializeObject(decodedEvent));

            return Task.FromResult(true);
        }
    }
}
