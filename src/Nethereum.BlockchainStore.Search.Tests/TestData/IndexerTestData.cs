using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using static Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract;

namespace Nethereum.BlockchainStore.Search.Tests.TestData
{
    public class IndexerTestData
    {
        public class SearchDocument : IHasId
        {
            string id;

            public SearchDocument()
            {
                id = GetHashCode().ToString();
            }

            public SearchDocument(string transactionHash, HexBigInteger logIndex)
            {
                TransactionHash = transactionHash;
                LogIndex = logIndex.Value.ToString();
                id = $"{TransactionHash}{logIndex.Value}";
            }

            public SearchDocument(TransactionForFunctionVO<TransferFunction> transactionForFunctionVO)
            {
                TransactionHash = transactionForFunctionVO.Transaction.TransactionHash;
                To = transactionForFunctionVO.FunctionMessage.To;
                Value = transactionForFunctionVO.FunctionMessage.Value.ToString();
                id = TransactionHash;
            }

            public SearchDocument(TransactionReceiptVO transactionReceiptVO)
            {
                TransactionReceiptVO = transactionReceiptVO;
                id = TransactionReceiptVO.TransactionHash;
            }

            public TransactionReceiptVO TransactionReceiptVO { get; }

            public string To { get; }
            public string Value { get; }

            public string TransactionHash { get; }
            public string LogIndex { get; }

            public string GetId()
            {
                return id;
            }
        }

        public static TransactionForFunctionVO<TransferFunction> CreateSampleTransactionForFunction()
        {
            Block block = new Block { Number = new HexBigInteger(new BigInteger(10)) };
            var tx = new Transaction
            {
                BlockNumber = block.Number,
                TransactionHash = "0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd",
                TransactionIndex = new HexBigInteger(new BigInteger(0))
            };

            var receipt = new TransactionReceipt { };
            var txWithReceipt = new TransactionReceiptVO(block, tx, receipt, false);
            var functionMessage = new TransferFunction
            {
                FromAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
                To = "0x22890d2cce102216644c59dae5baed380d84830c",
                Value = new BigInteger(101)
            };

            return new TransactionForFunctionVO<TransferFunction>(txWithReceipt, functionMessage);
        }

        public static TransactionReceiptVO CreateSampleTransaction()
        {
            Block block = new Block { Number = new HexBigInteger(new BigInteger(10)) };
            var tx = new Transaction
            {
                BlockNumber = block.Number,
                TransactionHash = "0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd",
                TransactionIndex = new HexBigInteger(new BigInteger(0))
            };

            var receipt = new TransactionReceipt { };
            return new TransactionReceiptVO(block, tx, receipt, false);
        }

    }
}
