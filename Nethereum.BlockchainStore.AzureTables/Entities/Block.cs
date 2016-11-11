using System.Numerics;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class Block : TableEntityBase
    {
        public Block(AzureTable at, DynamicTableEntity dte = null) : base(at, dte)
        {
            RowKey = string.Empty;
        }

        public string BlockNumber
        {
            get { return Get(string.Empty); }
            set
            {
                PartitionKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public string Hash
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string ParentHash
        {
            get { return Get(string.Empty); }
            set { Set(value, "ParentHash"); }
        }

        public string Nonce
        {
            get { return Get(string.Empty, "Nonce"); }
            set { Set(value, "Nonce"); }
        }

        public string ExtraData
        {
            get { return Get(string.Empty, "ExtraData"); }
            set { Set(value, "ExtraData"); }
        }

        public long Difficulty
        {
            get { return Get(0, "Difficulty"); }
            set { Set(value, "Difficulty"); }
        }

        public long TotalDifficulty
        {
            get { return Get(0, "TotalDifficulty"); }
            set { Set(value, "TotalDifficulty"); }
        }

        public long Size
        {
            get { return Get(0, "Size"); }
            set { Set(value, "Size"); }
        }

        public string Miner
        {
            get { return Get(string.Empty, "Miner"); }
            set { Set(value, "Miner"); }
        }

        public long GasLimit
        {
            get { return Get(0, "GasLimit"); }
            set { Set(value, "GasLimit"); }
        }

        public long GasUsed
        {
            get { return Get(0, "GasUsed"); }
            set { Set(value, "GasUsed"); }
        }

        public long TimeStamp
        {
            get { return Get(0, "TimeStamp"); }
            set { Set(value, "TimeStamp"); }
        }

        public long TransactionCount
        {
            get { return Get(0); }
            set { Set(value); }
        }

        public void SetBlockNumber(HexBigInteger blockNumber)
        {
            BlockNumber = blockNumber.Value.ToString();
        }

        public void SetDifficulty(HexBigInteger difficulty)
        {
            Difficulty = (long) difficulty.Value;
        }

        public void SetTotalDifficulty(HexBigInteger totalDifficulty)
        {
            TotalDifficulty = (long) totalDifficulty.Value;
        }

        public void SetTimeStamp(HexBigInteger timeStamp)
        {
            TimeStamp = (long) timeStamp.Value;
        }

        public void SetGasUsed(HexBigInteger gasUsed)
        {
            GasUsed = (long) gasUsed.Value;
        }

        public void SetGasLimit(HexBigInteger gasLimit)
        {
            GasLimit = (long) gasLimit.Value;
        }

        public void SetSize(HexBigInteger size)
        {
            Size = (long) size.Value;
        }

        public BigInteger GetBlockNumber()
        {
            var flag = string.IsNullOrEmpty(BlockNumber);
            BigInteger result;
            if (flag)
                result = default(BigInteger);
            else
                result = BigInteger.Parse(BlockNumber);
            return result;
        }
    }
}