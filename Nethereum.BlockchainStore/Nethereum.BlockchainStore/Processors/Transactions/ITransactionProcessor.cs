using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionProcessor
    {
        Task ProcessTransactionAsync(string transactionHash, BlockWithTransactionHashes block);
        bool EnabledContractCreationProcessing { get; set; }
        bool EnabledContractProcessing { get; set; }
        bool EnabledValueProcessing { get; set; }

        IContractTransactionProcessor ContractTransactionProcessor { get; }
    }
}