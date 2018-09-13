using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class Transaction : TransactionBase
    {

        public Transaction(string blockNumber, string hash)
        {
            BlockNumber = blockNumber;
            Hash = hash;
        }

        public override string Hash
        {
            get => RowKey;
            set => RowKey = value.ToRowKey();
        }

        public override string BlockNumber
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public override string AddressTo { get; set; } = string.Empty;

        public static Transaction CreateTransaction(
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            return
                (Transaction)
                CreateTransaction(new Transaction(transactionSource.BlockNumber.Value.ToString(), transactionSource.TransactionHash), transactionSource, transactionReceipt,
                    failed, timeStamp, error, hasVmStack);
        }


        public static Transaction CreateTransaction(
            RPC.Eth.DTOs.Transaction transactionSource,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, string newContractAddress)
        {
            return
                (Transaction)
                CreateTransaction(new Transaction(transactionSource.BlockNumber.Value.ToString(), transactionSource.TransactionHash), transactionSource, transactionReceipt,
                    failed, timeStamp, null, false, newContractAddress);
        }
    }
}