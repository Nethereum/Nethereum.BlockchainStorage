using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class BlockProgressRepository : AzureTableRepository<Entities.Counter>, 
        Nethereum.BlockchainProcessing.Processing.IBlockProgressRepository
    {
        private Entities.Counter _counter = new Entities.Counter{Name = "LastBlockProcessed", Value = -1};
        private bool _maxBlockInitialised;

        public BlockProgressRepository(CloudTable countersTable) : base(countersTable)
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            await InitialiseMaxBlock();
            if(_counter.Value < 0) return null;
            return (ulong)_counter.Value;
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            await InitialiseMaxBlock();
            _counter.Value = (long)blockNumber;
            await UpsertAsync(_counter, Table).ConfigureAwait(false);
        }

        private async Task InitialiseMaxBlock()
        {
            if (!_maxBlockInitialised)
            {
                var operation = TableOperation.Retrieve<Entities.Counter>(_counter.Name, "");
                var results = await Table.ExecuteAsync(operation);

                _counter = results.Result as Entities.Counter ?? _counter;
                _maxBlockInitialised = true;
            }
        }
    }
}
