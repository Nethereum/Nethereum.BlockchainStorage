using Nethereum.BlockchainProcessing.Storage.Entities;
using Newtonsoft.Json;

namespace Nethereum.BlockchainStore.CosmosCore.Entities
{
    public interface ICosmosEntity
    {
        string Id { get; }
    }

    public class CosmosTransaction: Transaction, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}";
            set => _id = value;
        }
    }

    public class CosmosAddressTransaction: AddressTransaction, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}{Address}";
            set => _id = value;
        }
    }

    public class CosmosBlockProgress : BlockProgress, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? LastBlockProcessed;
            set => _id = value;
        }
    }

    public class CosmosBlock : Block, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? BlockNumber;
            set => _id = value;
        }
    }

    public class CosmosContract : Contract, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? Address;
            set => _id = value;
        }
    }

    public class CosmosTransactionLog : TransactionLog, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? $"{TransactionHash}{LogIndex}";
            set => _id = value;
        }
    }

    public class CosmosTransactionVmStack : TransactionVmStack, ICosmosEntity
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get => _id ?? $"{TransactionHash}";
            set => _id = value;
        }
    }
}
