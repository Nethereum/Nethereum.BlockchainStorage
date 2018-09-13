﻿using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Net;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class AddressTransaction : TransactionBase
    {
        public AddressTransaction(string address)
        {
            Address = address;
        }

        public override string Hash { get; set; } = string.Empty;

        public string Address
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public override string AddressTo { get; set; } = string.Empty;

        //Store as a string so it can be parsed as a BigInteger
        public override string BlockNumber { get; set; } = string.Empty;

        public static AddressTransaction CreateAddressTransaction(
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp,
            string address,
            string error = null,
            bool hasVmStack = false,
            string newContractAddress = null
        )
        {
            var transaction = new AddressTransaction(address ?? string.Empty);
            transaction.SetRowKey(transactionSource.BlockNumber, transactionSource.TransactionHash);
            return
                (AddressTransaction)
                CreateTransaction(transaction, transactionSource, transactionReceipt, failed, timeStamp, error,
                    hasVmStack, newContractAddress);
        }

        public void SetRowKey(HexBigInteger blockNumber, string transactionHash)
        {
            RowKey = WebUtility.HtmlEncode(blockNumber.Value.ToString()) + "_" +
                    WebUtility.HtmlEncode(transactionHash.ToLowerInvariant());
        }
    }
}