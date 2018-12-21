using System;
using System.Threading.Tasks;
using Nethereum.Geth;
using Nethereum.Geth.RPC.Debug.DTOs;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public class BlockchainProxyService: IBlockchainProxyService
    {
        public BlockchainProxyService(string url):this(new Web3.Web3(url))
        {}

        public BlockchainProxyService(Web3.Web3 web3)
        {
            Web3 = web3;
        }

        private Web3.Web3 Web3 { get; }

        public Task<BlockWithTransactions> GetBlockWithTransactionsAsync
            (ulong blockNumber)
        {
            return Wrap(async () =>
            {
                var block =
                    await
                        Web3.Eth.Blocks.GetBlockWithTransactionsByNumber
                            .SendRequestAsync(
                            new HexBigInteger(blockNumber))
                            .ConfigureAwait(false);
                return block;
            });
        }

        public Task<string> GetCode(string address)
        {
            return Wrap(async () => await Web3.Eth.GetCode.SendRequestAsync(address)
                .ConfigureAwait(false));
        }

        public Task<FilterLog[]> GetLogs(NewFilterInput newFilter, object id = null)
        {
            return Wrap(async () => await Web3.Eth.Filters.GetLogs
                .SendRequestAsync(newFilter, id)
                .ConfigureAwait(false));
        }

        public Task<ulong> GetMaxBlockNumberAsync()
        {
            return Wrap(async () =>
            {
                var blockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
                return (ulong) blockNumber.Value;
            });
        }

        public Task<Transaction> GetTransactionByHash(string txnHash)
        {
            return Wrap(async () => await Web3.Eth.Transactions.GetTransactionByHash
                .SendRequestAsync(txnHash)
                .ConfigureAwait(false));
        }

        public Task<TransactionReceipt> GetTransactionReceipt(string txnHash)
        {
            return Wrap(async () => await Web3.Eth.Transactions.
                GetTransactionReceipt.
                SendRequestAsync(txnHash).
                ConfigureAwait(false));
        }

        public Task<JObject> GetTransactionVmStack(string transactionHash)
        {
            return Wrap(async () =>
            {
                if(Web3 is Web3Geth web3Geth)
                    return await web3Geth.Debug.
                        TraceTransaction.
                        SendRequestAsync(
                            transactionHash, 
                            new TraceTransactionOptions {DisableMemory = true, DisableStorage = true, DisableStack = false});

                return null;
            });
        }

        private async Task<T> Wrap<T>(Func<Task<T>> web3Request, int attempts = 1)
        {
            try
            {
                return await web3Request().ConfigureAwait(false);
            }
            catch (RpcClientUnknownException)
            {
                attempts++;

                if (attempts < 4)
                {
                    await Task.Delay(1000 * attempts);
                    return await Wrap(web3Request, attempts).ConfigureAwait(false);
                }

                throw;
            }

        }
    }
}
