using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface IContractTransactionProcessor
    {
        Task<bool> IsTransactionForContractAsync(Transaction transaction);

        Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt,
            HexBigInteger blockTimestamp);

        bool EnabledVmProcessing { get; set; }
    }
}