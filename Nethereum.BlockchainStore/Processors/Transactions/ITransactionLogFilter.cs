using Nethereum.BlockchainStore.Handlers;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionLogFilter: IFilter<TransactionLogWrapper>{}
}