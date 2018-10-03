﻿using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ContractCreationTransactionProcessorTests
    {
        public class ProcessTransactionAsync
        {
            private readonly Mock<IGetCode> _getCodeProxy = new Mock<IGetCode>();
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
                    .Verify(r => r.HandleAsync(It.IsAny<string>(), It.IsAny<string>(), transaction),
                        Times.Never);
            }

            private ContractCreationTransactionProcessor CreateProcessor()
            {
                return new ContractCreationTransactionProcessor(
                    _getCodeProxy.Object, _contractHandler.Object, _transactionHandler.Object);
            }

            private void EnsureNothingWasProcessed(Transaction transaction, TransactionReceipt receipt)
            {
                _getCodeProxy
                    .Verify(p => p.GetCode(It.IsAny<string>()),
                        Times.Never);

                _contractHandler
                    .Verify(r => r.HandleAsync(
                            It.IsAny<string>(), It.IsAny<string>(), transaction),
                        Times.Never);

                _transactionHandler
                    .Verify(r => r.HandleContractCreationTransactionAsync(receipt.ContractAddress, It.IsAny<string>(),
                            transaction, receipt, It.IsAny<bool>(), _blockTimestamp)
                        , Times.Never);
            }

            private void MockHandleTransaction(Transaction transaction, TransactionReceipt receipt, string code,
                bool failedContractCreation = false)
            {
                _transactionHandler
                    .Setup(r => r.HandleContractCreationTransactionAsync(
                        receipt.ContractAddress, code, transaction, receipt, failedContractCreation, _blockTimestamp))
                    .Returns(Task.CompletedTask).Verifiable();
            }

            private void MockHandleContract(Transaction transaction, TransactionReceipt receipt, string code)
            {
                _contractHandler
                    .Setup(r => r.HandleAsync(receipt.ContractAddress, code, transaction))
                    .Returns(Task.CompletedTask).Verifiable();
            }

            private void MockGetCode(TransactionReceipt receipt, string code)
            {
                _getCodeProxy
                    .Setup(p => p.GetCode(receipt.ContractAddress))
                    .ReturnsAsync(code);
            }

            private void EnsureContractHandlerWasInvoked()
            {
                _contractHandler.VerifyAll();
            }
        }
    }
}
