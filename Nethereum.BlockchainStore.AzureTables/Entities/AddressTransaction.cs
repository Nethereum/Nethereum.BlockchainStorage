using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class AddressTransaction : TransactionBase
    {
        public AddressTransaction(AzureTable azureTable, DynamicTableEntity dynamicTableEntity = null)
            : base(azureTable, dynamicTableEntity)
        {
        }

        public override string Hash
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Address
        {
            get { return Get(string.Empty); }
            set
            {
                PartitionKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public override string AddressTo
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        //Store as a string so it can be parsed as a BigInteger
        public override string BlockNumber
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public static AddressTransaction CreateAddressTransaction(AzureTable addressTransactionTable,
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp,
            string address,
            string error = null,
            bool hasVmStack = false,
            string newContractAddress = null
        )
        {
            var transaction = new AddressTransaction(addressTransactionTable) {Address = address ?? string.Empty};
            transaction.SetRowKey(transactionSource.BlockNumber, transactionSource.TransactionHash);
            return
                (AddressTransaction)
                CreateTransaction(transaction, transactionSource, transactionReceipt, failed, timeStamp, error,
                    hasVmStack, newContractAddress);
        }

        public void SetRowKey(HexBigInteger blockNumber, string transactionHash)
        {
            RowKey = blockNumber.Value.ToString().ToLowerInvariant().HtmlEncode() + "_" +
                     transactionHash.ToLowerInvariant().HtmlEncode();
        }
    }
}