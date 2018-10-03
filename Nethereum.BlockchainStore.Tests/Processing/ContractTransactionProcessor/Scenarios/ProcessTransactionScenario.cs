using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios
{
    public class ProcessTransactionScenario: ContractTransactionProcessorScenario
        {
            protected readonly string[] logAddresses = new [] { "address1", "address2" };

            protected readonly List<Tuple<string, long, JObject>> argsPassedToLogRepo = new List<Tuple<string, long, JObject>>();
            

            protected ProcessTransactionScenario()
            {
                 _receipt.Logs = JArray.FromObject(logAddresses.Select(a => new {address = a}));
            }

            protected void MockGetVmStack(JObject stackToReturn)
            {
                _vmStackProxy.Setup(p => p.GetTransactionVmStack(_transaction.TransactionHash))
                    .ReturnsAsync(stackToReturn);
            }

            protected void MockGetErrorFromVmStack(JObject vmStack, string errorToReturn)
            {
                _vmStackErrorChecker.Setup(e => e.GetError(vmStack)).Returns(errorToReturn);
            }

            protected void MockHandleVmStack(JObject vmStack)
            {
                _transactionVmStackHandler
                    .Setup(r => r.HandleAsync(_transaction.TransactionHash, _transaction.To, vmStack))
                    .Returns(Task.CompletedTask);
            }

            protected void MockHandleTransaction(bool hasError, bool hasStackTrace, string error)
            {
                _transactionHandler
                    .Setup(r => r.HandleTransactionAsync(_transaction, _receipt, hasError, _blockTimestamp, error, hasStackTrace))
                    .Returns(Task.CompletedTask);
            }

            protected void MockHandleAddressTransaction(bool hasError, bool hasStackTrace, string error)
            {
                foreach (var address in logAddresses)
                {
                    _transactionHandler.Setup(r =>
                            r.HandleAddressTransactionAsync(_transaction, _receipt, hasError, _blockTimestamp, address, error,
                                hasStackTrace))
                        .Returns(Task.CompletedTask);
                }
            }

            protected void MockHandleTransactionLog()
            {
                _transactionLogHandler
                    .Setup(r => r.HandleAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<JObject>()))
                    .Callback<string, long, JObject>((txnHash, idx, log) => { argsPassedToLogRepo.Add(new Tuple<string, long, JObject>(txnHash, idx, log)); })
                    .Returns(Task.CompletedTask);
            }

            protected void EnsureHandleTransactionWasInvoked()
            {
                _transactionHandler.VerifyAll();
            }

            protected void EnsureHandleVmStackWasInvoked()
            {
                _transactionVmStackHandler.VerifyAll();
            }

            protected void EnsureHandleTxLogWasInvoked()
            {
                Assert.Equal(logAddresses.Length, argsPassedToLogRepo.Count);

                for (var logIdx = 0; logIdx < 2; logIdx++)
                {
                    var args = argsPassedToLogRepo.FirstOrDefault(a => a.Item2 == logIdx);
                    Assert.NotNull(args);
                    Assert.Equal(logAddresses[logIdx], args.Item3["address"]);
                    Assert.Equal(_transaction.TransactionHash, args.Item1);
                }
            }

            protected void AddFilterWhichDoesNotMatchAnyLog()
            {
                _transactionLogFilters.Add(new TransactionLogFilter((log) => Task.FromResult(false)));
            }

            protected void EnsureHandleTxLogWasNotInvoked()
            {
                Assert.True(argsPassedToLogRepo.Count == 0);
            }

            protected void VerifyVmStackInfoHasNotBeenProcessed()
            {
                _vmStackProxy.Verify(p => p.GetTransactionVmStack(It.IsAny<string>()), Times.Never);
                _vmStackErrorChecker.Verify(e => e.GetError(It.IsAny<JObject>()), Times.Never());
                _transactionVmStackHandler
                    .Verify(r => r.HandleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JObject>()), Times.Never);
            }

            protected void MockExceptionFromGetTransactionVmStack()
            {
                _vmStackProxy.Setup(p => p.GetTransactionVmStack(_transaction.TransactionHash))
                    .Throws(new Exception("Fake GetVMStack exception"));
            }
        }
    }
