using System.Numerics;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public abstract class TransactionBase : TableEntity
    {
        private static readonly long minusOne = (long) -1;
        private static readonly long zero = (long) 0;

        public string BlockHash { get; set; } = string.Empty;
        public abstract string Hash { get; set; }
        public string AddressFrom { get; set; } = string.Empty;
        public long TimeStamp { get; set; } = minusOne;
        public long TransactionIndex { get; set; } = zero;
        public string Value { get; set; } = string.Empty;
        public abstract string AddressTo { get; set; }
        public abstract string BlockNumber { get; set; }
        public long Gas { get; protected set; } = zero;
        public long GasPrice { get; protected set; } = zero;
        public string Input { get; protected set; } = string.Empty;
        public long Nonce { get; set; } = zero;
        public bool Failed { get; set; } = false;
        public string ReceiptHash { get; protected set; } = string.Empty;
        public long GasUsed { get; protected set; } = zero;
        public long CumulativeGasUsed{ get; protected set; } = zero;
        public bool HasLog { get; protected set; } = false;
        public string Error{ get; protected set; } = string.Empty;
        public bool HasVmStack{ get; protected set; } = false;
        public string NewContractAddress{ get; set; } = string.Empty;
        public bool FailedCreateContract{ get; set; } = false;

        public static TransactionBase CreateTransaction(
            TransactionBase transaction,
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp,
            string error = null,
            bool hasVmStack = false,
            string newContractAddress = null)
        {
            transaction.BlockHash = transactionSource.BlockHash;
            transaction.Hash = transactionSource.TransactionHash;
            transaction.AddressFrom = transactionSource.From;
            transaction.TransactionIndex = (long) transactionReceipt.TransactionIndex.Value;
            transaction.SetValue(transactionSource.Value);
            transaction.AddressTo = transactionSource.To ?? string.Empty;
            transaction.NewContractAddress = newContractAddress ?? string.Empty;
            transaction.SetBlockNumber(transactionSource.BlockNumber);
            transaction.SetGas(transactionSource.Gas);
            transaction.SetGasPrice(transactionSource.GasPrice);
            transaction.Input = transaction.Input.RestrictToAzureTableStorageLimit(valIfTooLong: string.Empty);
            transaction.Nonce = (long) transactionSource.Nonce.Value;
            transaction.Failed = failed;
            transaction.SetGasUsed(transactionReceipt.GasUsed);
            transaction.SetCumulativeGasUsed(transactionReceipt.CumulativeGasUsed);
            transaction.HasLog = transactionReceipt.Logs.Count > 0;
            transaction.SetTimeStamp(timeStamp);
            transaction.Error = error ?? string.Empty;
            transaction.HasVmStack = hasVmStack;

            return transaction;
        }

        public void SetBlockNumber(HexBigInteger blockNumber)
        {
            BlockNumber = blockNumber.Value.ToString();
        }

        public void SetTimeStamp(HexBigInteger timeStamp)
        {
            TimeStamp = (long) timeStamp.Value;
        }

        public void SetGas(HexBigInteger gas)
        {
            Gas = (long) gas.Value;
        }

        public void SetGasUsed(HexBigInteger gasUsed)
        {
            GasUsed = (long) gasUsed.Value;
        }

        public void SetCumulativeGasUsed(HexBigInteger cumulativeGasUsed)
        {
            CumulativeGasUsed = (long) cumulativeGasUsed.Value;
        }

        public void SetGasPrice(HexBigInteger gasPrice)
        {
            GasPrice = (long) gasPrice.Value;
        }

        public void SetValue(HexBigInteger value)
        {
            Value = value.Value.ToString();
        }

        public BigInteger GetBlockNumber()
        {
            if (string.IsNullOrEmpty(BlockNumber)) return new BigInteger();
            return BigInteger.Parse(BlockNumber);
        }

        public BigInteger GetValue()
        {
            if (string.IsNullOrEmpty(Value)) return new BigInteger();
            return BigInteger.Parse(BlockNumber);
        }
    }
}