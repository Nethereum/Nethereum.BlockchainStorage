﻿using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionRepository : CosmosRepositoryBase, ITransactionRepository
    {
        public TransactionRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Transactions)
        {
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            var tx = new CosmosTransaction();
            tx.Map(transaction);
            tx.Map(transactionReceipt);

            tx.NewContractAddress = contractAddress;
            tx.Failed = false;
            tx.TimeStamp = (long)blockTimestamp.Value;
            tx.Error = string.Empty;
            tx.HasVmStack = false;

            tx.UpdateRowDates();

            await UpsertDocumentAsync(tx);
        }

        public async Task UpsertAsync(Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            var tx = new CosmosTransaction();
            tx.Map(transaction);
            tx.Map(receipt);

            tx.Failed = failed;
            tx.TimeStamp = (long)timeStamp.Value;
            tx.Error = error ?? string.Empty;
            tx.HasVmStack = hasVmStack;

            tx.UpdateRowDates();

            await UpsertDocumentAsync(tx);
        }
    }
}
