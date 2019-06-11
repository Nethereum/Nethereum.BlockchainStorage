using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace Nethereum.BlockchainStore.Search
{
    public class EventFunctionProcessor : IEventFunctionProcessor
    {
        private readonly IWeb3 _blockchainProxyService;
        private readonly Dictionary<Type, List<ITransactionHandler>> _eventToHandlerMapping;

        public EventFunctionProcessor(
            IWeb3 blockchainProxyService)
        {
            _blockchainProxyService = blockchainProxyService;
            _eventToHandlerMapping = new Dictionary<Type, List<ITransactionHandler>>();
        }

        public void AddHandler<TEventType>(ITransactionHandler handler)
        {
            if (!_eventToHandlerMapping.ContainsKey(typeof(TEventType)))
            {
                _eventToHandlerMapping.Add(typeof(TEventType), new List<ITransactionHandler>()); 
            }

            _eventToHandlerMapping[typeof(TEventType)].Add(handler);
        }

        public async Task ProcessAsync<TEvent>(EventLog<TEvent>[] logs)
        {
            if (!_eventToHandlerMapping.ContainsKey(typeof(TEvent))) return;

            var handlers = _eventToHandlerMapping[typeof(TEvent)];

            if (handlers.Count == 0) return;

            List<BlockWithTransactions> blockTransactions = await LoadTransactions(logs);

            foreach (var transactionHash in logs.Select(l => l.Log.TransactionHash).Distinct())
            {
                if (FindTransactionAndTimestamp(blockTransactions, transactionHash, out Transaction transaction,
                    out Hex.HexTypes.HexBigInteger blockTimestamp))
                {
                        await SendToHandler(handlers, transactionHash, transaction, blockTimestamp);
                }
            }
        }

        private async Task SendToHandler(List<ITransactionHandler> handlers, string transactionHash, Transaction transaction, Hex.HexTypes.HexBigInteger blockTimestamp)
        {
            var receipt = await _blockchainProxyService.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).ConfigureAwait(false);

            if (transaction.IsForContractCreation(receipt))
            {
                var code = await _blockchainProxyService.Eth.GetCode.SendRequestAsync(receipt.ContractAddress).ConfigureAwait(false);
                var contractCreationFailure = (code == null) || (code == "0x");
                var contactCreationTransaction = new ContractCreationTransaction(
                    receipt.ContractAddress,
                    code,
                    transaction,
                    receipt,
                    contractCreationFailure,
                    blockTimestamp);

                foreach (var handler in handlers)
                {
                    await handler.HandleContractCreationTransactionAsync(contactCreationTransaction);
                }

            }
            else
            {
                var txWithReceipt = new TransactionWithReceipt(
                    transaction,
                    receipt,
                    !receipt.Succeeded(),
                    blockTimestamp);

                foreach (var handler in handlers)
                {
                    await handler.HandleTransactionAsync(txWithReceipt);
                }
            }
        }

        private static bool FindTransactionAndTimestamp(List<BlockWithTransactions> blockTransactions, string transactionHash, out Transaction transaction, out Hex.HexTypes.HexBigInteger blockTimestamp)
        {
            var transactionAndBlock = blockTransactions.Select(b =>
            {
                return b.Transactions
                    .Where(t => t.TransactionHash.Equals(transactionHash, StringComparison.OrdinalIgnoreCase))
                    .Select(t => new { timestamp = b.Timestamp, transaction = t})
                    .FirstOrDefault();
            }).FirstOrDefault();

            transaction = null;
            blockTimestamp = null;

            if (transactionAndBlock?.transaction == null) return false;

            blockTimestamp = transactionAndBlock.timestamp;
            transaction = transactionAndBlock.transaction;
            
            return true;
        }

        private async Task<List<BlockWithTransactions>> LoadTransactions<TEvent>(EventLog<TEvent>[] logs)
        {
            var blockTransactions = new List<BlockWithTransactions>();

            foreach (var blockNumber in logs.Select(l => (ulong)l.Log.BlockNumber.Value).Distinct())
            {
                var blockWithTransactions = await _blockchainProxyService.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger()).ConfigureAwait(false);
                blockTransactions.Add(blockWithTransactions);
            }

            return blockTransactions;
        }

    }
}
