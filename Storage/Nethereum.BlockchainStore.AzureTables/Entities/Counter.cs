using Microsoft.WindowsAzure.Storage.Table;

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

        public long? Value { get; set; }
    }
}
