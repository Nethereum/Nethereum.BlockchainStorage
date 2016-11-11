using System.Numerics;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public abstract class TransactionBase : TableEntityBase
    {
        public TransactionBase(AzureTable azureTable, DynamicTableEntity dynamicTableEntity = null)
            : base(azureTable, dynamicTableEntity)
        {
        }

        public string BlockHash
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public abstract string Hash { get; set; }

        public string AddressFrom
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public long TimeStamp
        {
            get { return Get(-1); }
            set { Set(value); }
        }

        public long TransactionIndex
        {
            get { return Get(0); }
            set { Set(value); }
        }

        public string Value
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public abstract string AddressTo { get; set; }
        public abstract string BlockNumber { get; set; }

        public long Gas
        {
            get { return Get(0); }
            protected set { Set(value); }
        }

        public long GasPrice
        {
            get { return Get(0); }
            protected set { Set(value); }
        }

        public string Input
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public long Nonce
        {
            get { return Get(0); }
            set { Set(value); }
        }

        public bool Failed
        {
            get { return Get(false); }
            set { Set(value); }
        }

        public string ReceiptHash
        {
            get { return Get(string.Empty); }
            protected set { Set(value); }
        }

        public long GasUsed
        {
            get { return Get(0); }
            protected set { Set(value); }
        }

        public long CumulativeGasUsed
        {
            get { return Get(0); }
            protected set { Set(value); }
        }

        public bool HasLog
        {
            get { return Get(false); }
            protected set { Set(value); }
        }

        public string Error
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public bool HasVmStack
        {
            get { return Get(false); }
            set { Set(value); }
        }

        public string NewContractAddress
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public bool FailedCreateContract
        {
            get { return Get(false); }
            set { Set(value); }
        }

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
            transaction.Input = transactionSource.Input ?? string.Empty;
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