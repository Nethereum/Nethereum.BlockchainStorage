using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public class Web3Wrapper: 
        IGetBlockWithTransactionHashesByNumber,
        ITransactionProxy,
        IGetCode
    {
        public Web3Wrapper(Web3.Web3 web3)
        {
            Web3 = web3;
        }

        private Web3.Web3 Web3 { get; }

        public async Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashesByNumber(long blockNumber)
        {
            var block =
                await
                    Web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(
                        new HexBigInteger(blockNumber)).ConfigureAwait(false);
            return block;
        }

        public async Task<string> GetCode(string address)
        {
            return await Web3.Eth.GetCode.SendRequestAsync(address).ConfigureAwait(false);
        }

        public async Task<Transaction> GetTransactionByHash(string txnHash)
        {
            return await Web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txnHash).ConfigureAwait(false);
        }

        public async Task<TransactionReceipt> GetTransactionReceipt(string txnHash)
        {
            return await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txnHash).ConfigureAwait(false);
        }
    }
}
