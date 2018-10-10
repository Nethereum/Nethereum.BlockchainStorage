using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionProcessor
    {
        Task ProcessTransactionAsync(Block block, Transaction transaction);
        bool EnabledContractCreationProcessing { get; set; }
        bool EnabledContractProcessing { get; set; }
        bool EnabledValueProcessing { get; set; }

        IContractTransactionProcessor ContractTransactionProcessor { get; }
    }
}