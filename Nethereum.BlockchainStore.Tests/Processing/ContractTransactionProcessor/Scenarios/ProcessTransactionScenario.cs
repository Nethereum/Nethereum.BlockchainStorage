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

            protected void MockVmStackUpsert(JObject vmStack)
            {
                _transactionVmStackRepository
                    .Setup(r => r.UpsertAsync(_transaction.TransactionHash, _transaction.To, vmStack))
                    .Returns(Task.CompletedTask);
            }

            protected void MockTransactionUpsert(bool hasError, bool hasStackTrace, string error)
            {
                _transactionRepository
                    .Setup(r => r.UpsertAsync(_transaction, _receipt, hasError, _blockTimestamp, hasStackTrace, error))
                    .Returns(Task.CompletedTask);
            }

            protected void MockAddressTransactionUpsert(bool hasError, bool hasStackTrace, string error)
            {
                foreach (var address in new []{_transaction.To}.Concat(logAddresses))
                {
                    _addressTransactionRepository.Setup(r =>
                            r.UpsertAsync(_transaction, _receipt, hasError, _blockTimestamp, address, error,
                                hasStackTrace, null))
                        .Returns(Task.CompletedTask);
                }
            }

            protected void MockTransactionLogUpserts()
            {
                _transactionLogRepository
                    .Setup(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<JObject>()))
                    .Callback<string, long, JObject>((txnHash, idx, log) => { argsPassedToLogRepo.Add(new Tuple<string, long, JObject>(txnHash, idx, log)); })
                    .Returns(Task.CompletedTask);
            }

            protected void VerifyAddressTransactionUpsert()
            {
                _addressTransactionRepository.VerifyAll();
            }

            protected void VerifyTransactionUpsert()
            {
                _transactionRepository.VerifyAll();
            }

            protected void VerifyVmStackUpsert()
            {
                _transactionVmStackRepository.VerifyAll();
            }

            protected void VerifyTxLogUpserts()
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

            protected void VerifyNoLogsHaveBeenUpserted()
            {
                Assert.True(argsPassedToLogRepo.Count == 0);
            }

            protected void VerifyVmStackInfoHasNotBeenProcessed()
            {
                _vmStackProxy.Verify(p => p.GetTransactionVmStack(It.IsAny<string>()), Times.Never);
                _vmStackErrorChecker.Verify(e => e.GetError(It.IsAny<JObject>()), Times.Never());
                _transactionVmStackRepository
                    .Verify(r => r.UpsertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JObject>()), Times.Never);
            }

            protected void MockExceptionFromGetTransactionVmStack()
            {
                _vmStackProxy.Setup(p => p.GetTransactionVmStack(_transaction.TransactionHash))
                    .Throws(new Exception("Fake GetVMStack exception"));
            }
        }
    }
