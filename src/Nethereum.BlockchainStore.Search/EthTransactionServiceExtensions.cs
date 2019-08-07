using Nethereum.Contracts.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public static class EthTransactionServiceExtensions
    {
        public static async Task<TransactionReceiptVO> GetTransactionReceiptVO(this IEthApiContractService eth, HexBigInteger blockNumber, string transactionHash)
        {
            var tx = eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var receipt = eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var block = eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(blockNumber);

            await Task.WhenAll(tx, receipt, block).ConfigureAwait(false);

            return new TransactionReceiptVO(block.Result, tx.Result, receipt.Result, receipt.Result.HasErrors() ?? false);
        }
    }
}
