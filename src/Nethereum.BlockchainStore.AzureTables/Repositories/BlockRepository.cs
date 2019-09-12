using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
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

            blockOutput.TransactionCount = GetTransactionCount(blockSource);

            return blockOutput;
        }

        public static int GetTransactionCount(RPC.Eth.DTOs.Block block)
        {
            if (block is BlockWithTransactions b)
                return b.Transactions?.Length ?? 0;

            if (block is BlockWithTransactionHashes bh)
                return bh.TransactionHashes?.Length ?? 0;

            return 0;
        }


        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var operation = TableOperation.Retrieve<Block>(blockNumber.Value.ToString(), "");
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Block;
        }

    }

}