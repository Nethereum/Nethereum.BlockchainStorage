using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.AzureTables.Entities.Block;


namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class BlockRepository : AzureTableRepository<Block>, IBlockRepository
    {
        public BlockRepository(CloudTable table) : base(table)
        {
        }

        public async Task UpsertBlockAsync(BlockWithTransactionHashes source)
        {
            var blockEntity = MapBlock(source, new Block(source.Number.Value.ToString()));
            await UpsertAsync(blockEntity);
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

        //TODO: Implement 
        public Task<long> GetMaxBlockNumber()
        {
            return Task.FromResult((long)0);
        }

        public async Task<Block> GetBlockAsync(HexBigInteger blockNumber)
        {
            var operation = TableOperation.Retrieve<Block>(blockNumber.Value.ToString(), "");
            var results = await _table.ExecuteAsync(operation);
            return results.Result as Block;
        }


    }
}