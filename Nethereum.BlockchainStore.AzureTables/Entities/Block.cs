using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class Block : TableEntity, IBlockView
    {
        public Block()
        {
        }

        public Block(string blockNumber)
        {
            BlockNumber = blockNumber;
            RowKey = string.Empty;
        }

        public string BlockNumber
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public string Hash { get; set; } = string.Empty;
        public string ParentHash { get; set; } = string.Empty;
        public long Nonce { get; set; } = 0;
        public string ExtraData { get; set; } = string.Empty;
        public long Difficulty { get; set; } = 0;
        public long TotalDifficulty { get; set; } = 0;
        public long Size { get; set; } = 0;
        public string Miner { get; set; } = string.Empty;
        public long GasLimit { get; set; } = 0;
        public long GasUsed { get; set; } = 0;
        public long TimeStamp { get; set; } = 0;
        public long TransactionCount { get; set; } = 0;

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