using System.Numerics;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public abstract class TransactionBase : TableEntity
    {
        public string BlockHash { get; set; } = string.Empty;
        public abstract string Hash { get; set; }
        public string AddressFrom { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
        public string TransactionIndex { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public abstract string AddressTo { get; set; }
        public abstract string BlockNumber { get; set; }
        public string Gas { get; set; } = string.Empty;
        public string GasPrice { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string Nonce { get; set; } = string.Empty;
        public bool Failed { get; set; } = false;
        public string ReceiptHash { get; set; } = string.Empty;
        public string GasUsed { get; set; } = string.Empty;
        public string CumulativeGasUsed { get; set; } = string.Empty;
        public bool HasLog { get; set; } = false;
        public string Error{ get; set; } = string.Empty;
        public bool HasVmStack{ get; set; } = false;
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
            transaction.TransactionIndex = transactionReceipt.TransactionIndex.Value.ToString();
            transaction.SetValue(transactionSource.Value);
            transaction.AddressTo = transactionSource.To ?? string.Empty;
            transaction.NewContractAddress = newContractAddress ?? string.Empty;
            transaction.SetBlockNumber(transactionSource.BlockNumber);
            transaction.SetGas(transactionSource.Gas);
            transaction.SetGasPrice(transactionSource.GasPrice);
            transaction.Input = transactionSource.Input.RestrictToAzureTableStorageLimit(valIfTooLong: string.Empty);
            transaction.Nonce = transactionSource.Nonce.Value.ToString();
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
            TimeStamp = timeStamp.Value.ToString();
        }

        public void SetGas(HexBigInteger gas)
        {
            Gas = gas.Value.ToString();
        }

        public void SetGasUsed(HexBigInteger gasUsed)
        {
            GasUsed = gasUsed.Value.ToString();
        }

        public void SetCumulativeGasUsed(HexBigInteger cumulativeGasUsed)
        {
            CumulativeGasUsed = cumulativeGasUsed.Value.ToString();
        }

        public void SetGasPrice(HexBigInteger gasPrice)
        {
            GasPrice = gasPrice.Value.ToString();
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