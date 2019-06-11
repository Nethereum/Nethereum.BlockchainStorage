using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.RPC.Eth.Transactions;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class GetTransactionEventHandler : EventHandlerBase, IEventHandler
    {
        public GetTransactionEventHandler(IEventSubscription subscription, long id, IEthGetTransactionByHash proxy)
            :base(subscription, id)
        {
            GetTransactionProxy = proxy;
        }

        public IEthGetTransactionByHash GetTransactionProxy { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            decodedEvent.Transaction = await GetTransactionProxy.SendRequestAsync(decodedEvent.Log.TransactionHash).ConfigureAwait(false);
            return true;
        }
    }
}