using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class BlockProgressRepository : AzureTableRepository<Entities.Counter>, 
        IBlockProgressRepository
    {
        private Entities.Counter _counter = new Entities.Counter{Name = "LastBlockProcessed", Value = null};
        private bool _maxBlockInitialised;

        public BlockProgressRepository(CloudTable countersTable) : base(countersTable)
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            await InitialiseMaxBlock();
            if(_counter == null) return null;
            if(_counter.Value == null) return null;
            return BigInteger.Parse(_counter.Value);
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            await InitialiseMaxBlock();
            _counter.Value = blockNumber.ToString();
            await UpsertAsync(_counter, Table).ConfigureAwait(false);
        }

        private async Task InitialiseMaxBlock()
        {
            if (!_maxBlockInitialised)
            {
                var operation = TableOperation.Retrieve<Entities.Counter>(_counter.Name, "");
                var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);

                _counter = results.Result as Entities.Counter ?? _counter;
                _maxBlockInitialised = true;
            }
        }
    }
}
