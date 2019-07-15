using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Block = Nethereum.BlockchainStore.AzureTables.Entities.Block;


namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class BlockRepository : AzureTableRepository<Block>, IBlockRepository
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public BlockRepository(CloudTable table) : base(table)
        {
        }

        public async Task UpsertBlockAsync(RPC.Eth.DTOs.Block source)
        {
            await _lock.WaitAsync();
            try
            {
                var blockEntity = MapBlock(source, new Block(source.Number.Value.ToString()));
                await UpsertAsync(blockEntity).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }


        public Block MapBlock(RPC.Eth.DTOs.Block blockSource, Block blockOutput)
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
            blockOutput.Miner = blockSource.Miner ?? string.Empty;
            blockOutput.Nonce = blockSource.Nonce ?? string.Empty;

            blockOutput.TransactionCount = blockSource.TransactionCount();

            return blockOutput;
        }


        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var operation = TableOperation.Retrieve<Block>(blockNumber.Value.ToString(), "");
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Block;
        }

    }
}