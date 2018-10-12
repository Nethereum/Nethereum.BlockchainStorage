using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionLogProcessor
    {
        Task ProcessAsync(Transaction transaction, TransactionReceipt receipt);
    }
}