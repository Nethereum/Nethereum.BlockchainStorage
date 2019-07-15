using Microsoft.WindowsAzure.Storage.Table;
using System.Numerics;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class Counter: TableEntity
    {
        public Counter()
        {
            RowKey = string.Empty;
        }

        public string Name
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public string Value { get; set; }
    }
}
