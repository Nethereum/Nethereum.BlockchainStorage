using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Processors;
using Nethereum.RPC.Eth.DTOs;
using Wintellect.Azure.Storage.Table;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        protected AzureTable BlockTable { get; set; }

        public BlockRepository(CloudTable blockCloudTable)
        {
            BlockTable = new AzureTable(blockCloudTable);
        }

        public async Task UpsertBlockAsync(BlockWithTransactionHashes block)
        {
            var blockEntity = MapBlock(block, new Block(BlockTable));
            await blockEntity.InsertOrReplaceAsync().ConfigureAwait(false);
        }

        public Block MapBlock(BlockWithTransactionHashes blockSource, Block blockOutput)
        {
            blockOutput.SetBlockNumber(blockSource.Number);
            blockOutput.SetDifficulty(blockSource.Difficulty);
            blockOutput.SetGasLimit(blockSource.GasLimit);
            blockOutput.SetGasUsed(blockSource.GasUsed);
            blockOutput.SetSize(blockSource.Size);
            blockOutput.SetTimeStamp(blockSource.Timestamp);
            blockOutput.SetTotalDifficulty(blockSource.TotalDifficulty);
            blockOutput.ExtraData = blockSource.ExtraData ?? string.Empty;
            blockOutput.Hash = blockSource.BlockHash ?? string.Empty;
            blockOutput.ParentHash = blockSource.ParentHash ?? string.Empty;
            blockOutput.Miner = blockOutput.Miner ?? string.Empty;
            blockOutput.Nonce = blockOutput.Nonce ?? string.Empty;
            blockOutput.TransactionCount = blockSource.TransactionHashes.Length;

            return blockOutput;
        }
    }
}