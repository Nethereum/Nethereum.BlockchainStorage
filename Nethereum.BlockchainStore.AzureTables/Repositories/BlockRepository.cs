using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.AzureTables.Entities.Block;


namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class BlockRepository : AzureTableRepository<Block>, IBlockRepository
    {
        private bool _maxBlockInitialised = false;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private Counter _maxBlockCounter = new Counter() {Name = "MaxBlockNumber", Value = 0};
        private readonly CloudTable _countersTable;

        public BlockRepository(CloudTable table, CloudTable countersTable) : base(table)
        {
            _countersTable = countersTable;
        }

        public async Task UpsertBlockAsync(RPC.Eth.DTOs.Block source)
        {
            await _lock.WaitAsync();
            try
            {
                await InitialiseMaxBlock();

                var blockEntity = MapBlock(source, new Block(source.Number.Value.ToString()));
                await UpsertAsync(blockEntity);
                await UpdateMaxBlockNumber(blockEntity);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task UpdateMaxBlockNumber(Block blockEntity)
        {
            var blockNumber = long.Parse(blockEntity.BlockNumber);

            if (blockNumber > _maxBlockCounter.Value)
            {
                _maxBlockCounter.Value = blockNumber;
                await UpsertAsync(_maxBlockCounter, _countersTable).ConfigureAwait(false);
            }
        }

        private async Task InitialiseMaxBlock()
        {
            if (!_maxBlockInitialised)
            {
                var operation = TableOperation.Retrieve<Counter>(_maxBlockCounter.Name, "");
                var results = await _countersTable.ExecuteAsync(operation);

                _maxBlockCounter = results.Result as Counter ?? _maxBlockCounter;
                _maxBlockInitialised = true;
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
            blockOutput.Nonce = string.IsNullOrEmpty(blockSource.Nonce) ? 0 : (long)new HexBigInteger(blockSource.Nonce).Value;

            blockOutput.TransactionCount = blockSource.TransactionCount();

            return blockOutput;
        }

        public async Task<long> GetMaxBlockNumberAsync()
        {
            await _lock.WaitAsync();
            try
            {
                await InitialiseMaxBlock();
                return _maxBlockCounter.Value;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var operation = TableOperation.Retrieve<Block>(blockNumber.Value.ToString(), "");
            var results = await Table.ExecuteAsync(operation);
            return results.Result as Block;
        }


    }
}