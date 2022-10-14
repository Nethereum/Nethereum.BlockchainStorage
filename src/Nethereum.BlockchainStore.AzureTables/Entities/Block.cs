using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.Hex.HexTypes;
using System.Numerics;

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
        public string Nonce { get; set; } = string.Empty;
        public string ExtraData { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string TotalDifficulty { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Miner { get; set; } = string.Empty;
        public string GasLimit { get; set; } = string.Empty;
        public string GasUsed { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
        public long TransactionCount { get; set; } = 0;
        public string BaseFeePerGas {get; set;} = string.Empty;

        public void SetBlockNumber(HexBigInteger blockNumber)
        {
            BlockNumber = blockNumber.Value.ToString();
        }

        public void SetDifficulty(HexBigInteger difficulty)
        {
            Difficulty = difficulty.Value.ToString();
        }

        public void SetTotalDifficulty(HexBigInteger totalDifficulty)
        {
            TotalDifficulty = totalDifficulty.Value.ToString();
        }

        public void SetTimeStamp(HexBigInteger timeStamp)
        {
            TimeStamp = timeStamp.Value.ToString();
        }

        public void SetGasUsed(HexBigInteger gasUsed)
        {
            GasUsed = gasUsed.Value.ToString();
        }

        public void SetGasLimit(HexBigInteger gasLimit)
        {
            GasLimit = gasLimit.Value.ToString();
        }

        public void SetSize(HexBigInteger size)
        {
            Size = size.Value.ToString();
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