using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.MongoDb.Entities
{
    public interface IMongoDbEntity
    {
        string Id { get; }
    }

    public class MongoDbTransaction: Transaction, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}";
            set => _id = value;
        }
    }

    public class MongoDbAddressTransaction: AddressTransaction, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}{Address}";
            set => _id = value;
        }
    }

    public class MongoDbBlock : Block, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? BlockNumber;
            set => _id = value;
        }
    }

    public class MongoDbContract : Contract, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? Address;
            set => _id = value;
        }
    }

    public class MongoDbTransactionLog : TransactionLog, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{TransactionHash}{LogIndex}";
            set => _id = value;
        }
    }

    public class MongoDbTransactionVmStack : TransactionVmStack, IMongoDbEntity
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{TransactionHash}";
            set => _id = value;
        }
    }
}
