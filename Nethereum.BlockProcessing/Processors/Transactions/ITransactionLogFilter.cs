using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public interface ITransactionLogFilter: IFilter<TransactionLogWrapper>{}
}