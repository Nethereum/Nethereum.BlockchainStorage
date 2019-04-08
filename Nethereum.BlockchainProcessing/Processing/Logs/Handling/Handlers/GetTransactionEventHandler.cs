using Nethereum.BlockchainProcessing.BlockchainProxy;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class GetTransactionEventHandler : EventHandlerBase, IEventHandler
    {
        public GetTransactionEventHandler(IEventSubscription subscription, long id, IGetTransactionByHash proxy)
            :base(subscription, id)
        {
            Proxy = proxy;
        }

        public IGetTransactionByHash Proxy { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            decodedEvent.Transaction = await Proxy.GetTransactionByHash(decodedEvent.Log.TransactionHash);
            return true;
        }
    }
}