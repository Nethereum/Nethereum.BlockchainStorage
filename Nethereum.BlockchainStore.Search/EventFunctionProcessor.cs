using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public class EventFunctionProcessor<TFunction> : IEventFunctionProcessor<TFunction>
        where TFunction : FunctionMessage, new()
    {
        public EventFunctionProcessor(
            IBlockchainProxyService blockchainProxyService, 
            ITransactionHandler<TFunction> handler)
        {
            BlockchainProxyService = blockchainProxyService;
            Handler = handler;
        }

        public IBlockchainProxyService BlockchainProxyService { get; }
        public ITransactionHandler<TFunction> Handler { get; }
    
        public async Task Process(FilterLog[] logs)
        {
            List<BlockWithTransactions> blockTransactions = await LoadTransactions(logs);

            foreach (var transactionHash in logs.Select(l => l.TransactionHash).Distinct())
            {
                if (FindTransactionAndTimestamp(blockTransactions, transactionHash, out Transaction transaction,
                    out Hex.HexTypes.HexBigInteger blockTimestamp))
                {
                    if (transaction.IsTransactionForFunctionMessage<TFunction>())
                    {
                        await SendToHandler(transactionHash, transaction, blockTimestamp);
                    }
                }
            }
        }

        private async Task SendToHandler(string transactionHash, Transaction transaction, Hex.HexTypes.HexBigInteger blockTimestamp)
        {
            var receipt = await BlockchainProxyService.GetTransactionReceipt(transactionHash);

            if (transaction.IsForContractCreation(receipt))
            {
                var code = await BlockchainProxyService.GetCode(receipt.ContractAddress);
                var contractCreationFailure = (code == null) || (code == "0x");
                var contactCreationTransaction = new ContractCreationTransaction(
                    receipt.ContractAddress,
                    code,
                    transaction,
                    receipt,
                    contractCreationFailure,
                    blockTimestamp);

                await Handler.HandleContractCreationTransactionAsync(contactCreationTransaction);

            }
            else
            {
                var txWithReceipt = new TransactionWithReceipt(
                    transaction,
                    receipt,
                    !receipt.Succeeded(),
                    blockTimestamp);

                await Handler.HandleTransactionAsync(txWithReceipt);
            }
        }

        private static bool FindTransactionAndTimestamp(List<BlockWithTransactions> blockTransactions, string transactionHash, out Transaction transaction, out Hex.HexTypes.HexBigInteger blockTimestamp)
        {
            var transactionAndBlock = blockTransactions.Select(b =>
            {
                return b.Transactions
                    .Where(t => t.TransactionHash.Equals(transactionHash, StringComparison.OrdinalIgnoreCase))
                    .Select(t => (b, t))
                    .FirstOrDefault();
            }).FirstOrDefault();

            transaction = null;
            blockTimestamp = null;

            if (transactionAndBlock.Item1 == null) return false;

            transaction = transactionAndBlock.Item2;
            blockTimestamp = transactionAndBlock.Item1.Timestamp;

            return true;
        }

        private async Task<List<BlockWithTransactions>> LoadTransactions(FilterLog[] logs)
        {
            var blockTransactions = new List<BlockWithTransactions>();

            foreach (var blockNumber in logs.Select(l => (ulong)l.BlockNumber.Value).Distinct())
            {
                var blockWithTransactions = await BlockchainProxyService.GetBlockWithTransactionsAsync(blockNumber);
                blockTransactions.Add(blockWithTransactions);
            }

            return blockTransactions;
        }

    }
}
