using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class Transaction : TransactionBase
    {
        public Transaction(AzureTable azureTable, DynamicTableEntity dynamicTableEntity = null)
            : base(azureTable, dynamicTableEntity)
        {
        }

        public override string Hash
        {
            get { return Get(string.Empty); }
            set
            {
                RowKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public override string BlockNumber
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

        public static Transaction CreateTransaction(AzureTable transactionTable,
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            return
                (Transaction)
                CreateTransaction(new Transaction(transactionTable), transactionSource, transactionReceipt,
                    failed, timeStamp, error, hasVmStack);
        }


        public static Transaction CreateTransaction(AzureTable transactionTable,
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, string newContractAddress)
        {
            return
                (Transaction)
                CreateTransaction(new Transaction(transactionTable), transactionSource, transactionReceipt,
                    failed, timeStamp, null, false, newContractAddress);
        }
    }
}