using Moq;
using Nethereum.BlockchainProcessing.Common.Tests;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockProcessing.Tests
{
    public class ContractCreationTransactionProcessorTests
    {
        public class ProcessTransactionAsync
        {
            private readonly Web3Mock _web3Mock = new Web3Mock();
            private readonly Mock<IContractHandler> _contractHandler = new Mock<IContractHandler>();
            private readonly Mock<ITransactionHandler> _transactionHandler = new Mock<ITransactionHandler>();
            private ContractCreationTransactionProcessor _processor;

            private readonly HexBigInteger _blockTimestamp =
                new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            [Fact]
            public async Task Ignores_Non_Contract_Creation_Transactions()
            {
                var transaction = new Transaction
                {
                    To = "0x1009b29f2094457d3dba62d1953efea58176ba28"
                };
                var receipt = new TransactionReceipt();

                _processor = CreateProcessor();

                await _processor.ProcessTransactionAsync(
                    transaction, receipt, _blockTimestamp);

                EnsureNothingWasProcessed(transaction, receipt);
            }

            [Fact]
            public async Task Processes_Valid_Transaction()
            {
                var transaction = new Transaction {To = ""};
                var receipt = new TransactionReceipt
                {
                    ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
                };
                const string Code = "codecodecodecodeetcetc";

                MockGetCode(receipt, Code);
                MockHandleContract(transaction, receipt, Code);
                MockHandleTransaction(transaction, receipt, Code);

                _processor = CreateProcessor();

                await _processor.ProcessTransactionAsync(
                    transaction, receipt, _blockTimestamp);

                EnsureContractHandlerWasInvoked();
                EnsureTransactionHandlerWasInvoked();
            }

            [Theory]
            [InlineData("0x")]
            [InlineData(null)]
            public async Task
                Processes_Failed_Contract_Creation_Transaction_But_Does_Not_Invoke_ContractHandler
                (string code)
            {
                var transaction = new Transaction {To = ""};
                var receipt = new TransactionReceipt
                {
                    ContractAddress = "0x1009b29f2094457d3dba62d1953efea58176ba27"
                };

                MockGetCode(receipt, code);

                const bool failedContractCreation = true;

                MockHandleTransaction(transaction, receipt, code, failedContractCreation);

                _processor = CreateProcessor();

                await _processor.ProcessTransactionAsync(
                    transaction, receipt, _blockTimestamp);

                EnsureContractHandlerWasNotInvoked(transaction);

                EnsureTransactionHandlerWasInvoked();
            }

            private void EnsureTransactionHandlerWasInvoked()
            {
                _transactionHandler.VerifyAll();
            }

            private void EnsureContractHandlerWasNotInvoked(Transaction transaction)
            {
                _contractHandler
                    .Verify(r => r.HandleAsync(new ContractTransaction(It.IsAny<string>(), It.IsAny<string>(), transaction)),
                        Times.Never);
            }

            private ContractCreationTransactionProcessor CreateProcessor()
            {
                return new ContractCreationTransactionProcessor(
                    _web3Mock.Web3, _contractHandler.Object, _transactionHandler.Object);
            }

            private void EnsureNothingWasProcessed(Transaction transaction, TransactionReceipt receipt)
            {
                _web3Mock.GetCodeMock
                    .Verify(p => p.SendRequestAsync(It.IsAny<string>(), (object)null),
                        Times.Never);

                _contractHandler
                    .Verify(r => r.HandleAsync(It.IsAny<ContractTransaction>()),
                        Times.Never);

                _transactionHandler
                    .Verify(r => r.HandleContractCreationTransactionAsync(It.IsAny<ContractCreationTransaction>())
                        , Times.Never);
            }

            private void MockHandleTransaction(Transaction transaction, TransactionReceipt receipt, string code,
                bool failedContractCreation = false)
            {
                _transactionHandler
                    .Setup(r => r.HandleContractCreationTransactionAsync( It.IsAny<ContractCreationTransaction>()))
                    .Callback<ContractCreationTransaction>((ct) =>
                    {
                        Assert.Equal(transaction, ct.Transaction);
                        Assert.Equal(receipt, ct.TransactionReceipt);
                        Assert.Equal(code, ct.Code);
                        Assert.Equal(failedContractCreation, ct.FailedCreatingContract);
                    })
                    .Returns(Task.CompletedTask).Verifiable();
            }

            private void MockHandleContract(Transaction transaction, TransactionReceipt receipt, string code)
            {
                _contractHandler
                    .Setup(r => r.HandleAsync(It.IsAny<ContractTransaction>()))
                    .Callback<ContractTransaction>((ct) =>
                    {
                        Assert.Equal(transaction, ct.Transaction);
                        Assert.Equal(code, ct.Code);
                        Assert.Equal(ct.ContractAddress, receipt.ContractAddress);
                    })
                    .Returns(Task.CompletedTask).Verifiable();
            }

            private void MockGetCode(TransactionReceipt receipt, string code)
            {
                _web3Mock.GetCodeMock
                    .Setup(p => p.SendRequestAsync(receipt.ContractAddress, (object)null))
                    .ReturnsAsync(code);
            }

            private void EnsureContractHandlerWasInvoked()
            {
                _contractHandler.VerifyAll();
            }
        }
    }
}
