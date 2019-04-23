using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Numerics;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.DtoWrappers
{
    public class TransactionWithReceiptTests
    {
        [Function("doSomething")]
        public class DoSomethingFunction: FunctionMessage{}
        
        [Fact]
        public void WhenTxIsNullBlockNumberIsZero()
        {
            var txWithReceipt = new TransactionWithReceipt();
            Assert.Equal(BigInteger.Zero, txWithReceipt.BlockNumber.Value);
        }

        /// <summary>
        /// Ensuring that the various convenience properties and methods on this wrapper object
        /// are present and return expected values
        /// </summary>
        [Fact]
        public void Convenience_Properties_And_Methods()
        {
            var txn = new Transaction
            {
                BlockNumber = new HexBigInteger(100),
                TransactionHash = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9",
                From = "0x1009b29f2094457d3dba62d1953efea58176ba27",
                To = "0x2009b29f2094457d3dba62d1953efea58176ba27",
                Value = new HexBigInteger(0),
                Gas = new HexBigInteger(0),
                GasPrice = new HexBigInteger(0),
                Nonce = new HexBigInteger(0)
            };

            const string logAddress = "0x4009b29f2094457d3dba62d1953efea58176ba27";

            var receipt = new TransactionReceipt
            {
                ContractAddress = "0x3009b29f2094457d3dba62d1953efea58176ba27",
                Status = new HexBigInteger(BigInteger.One),
                Logs = JArray.FromObject(new []{ new{address = logAddress}})
            };

            var f = new DoSomethingFunction
            {
                Gas = txn.Gas,
                GasPrice = txn.GasPrice,
                Nonce = txn.Nonce,
                FromAddress = txn.From,
                AmountToSend = txn.Value
            };

            var txInput = f.CreateTransactionInput(receipt.ContractAddress);
            txn.Input = txInput.Data;

            // this is a workaround for a bug in FunctionCallDecoder v3.00rc3
            // if sig == data it returned a null instead of the function input object
            // adding this zero prevents it from doing that
            // bug already fixed in Neth awaiting next release
            txn.Input = txn.Input + "0";

            var txnWithReceipt = new TransactionWithReceipt(txn, receipt, false, null);

            Assert.Equal(txn.BlockNumber, txnWithReceipt.BlockNumber);
            Assert.Equal(txn.TransactionHash, txnWithReceipt.TransactionHash);
            Assert.True(txnWithReceipt.Succeeded);
            Assert.False(txnWithReceipt.Failed);
            Assert.True(txnWithReceipt.HasLogs());

            var relatedAddresses = txnWithReceipt.GetAllRelatedAddresses();
            Assert.Contains(txn.From, relatedAddresses);
            Assert.Contains(txn.To, relatedAddresses);
            Assert.Contains(receipt.ContractAddress, relatedAddresses);
            Assert.Contains(logAddress, relatedAddresses);

            receipt.Status = new HexBigInteger(BigInteger.Zero);
            Assert.False(txnWithReceipt.Succeeded);
            Assert.True(txnWithReceipt.Failed);

            Assert.True(txnWithReceipt.IsForFunction<DoSomethingFunction>());
            Assert.NotNull(txnWithReceipt.Decode<DoSomethingFunction>());
        }
    }
}
