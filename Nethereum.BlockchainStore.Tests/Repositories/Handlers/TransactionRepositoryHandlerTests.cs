using Moq;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Repositories.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Repositories.Handlers
{
    public class TransactionRepositoryHandlerTests
    {
        protected readonly Mock<ITransactionRepository> _transactionRepository = new Mock<ITransactionRepository>();
        protected readonly Mock<IAddressTransactionRepository> _addressTransactionRepository = new Mock<IAddressTransactionRepository>();
        protected readonly TransactionRepositoryHandler _handler;

        protected int _txRepoUpsertCount = 0;
        protected int _addressRepoUpsertCount = 0;

        protected const string LogAddress1 = "0x1009b29f2094457d3dba62d1953efea58176ba27";
        protected const string LogAddress2 = "0x2009b29f2094457d3dba62d1953efea58176ba27";

        public TransactionRepositoryHandlerTests()
        {
            _handler = new TransactionRepositoryHandler(_transactionRepository.Object, _addressTransactionRepository.Object);
        }

        [Fact]
        public async Task HandleTransactionAsync_Calls_Tx_Repo_Once_And_Address_Repo_For_Each_Associated_Address()
        {
            var tx = CreateTransaction();
            var receipt = CreateReceipt();
            TransactionWithReceipt txWithReceipt = CreateTransactionWithReceipt(tx, receipt);

            SetUpTransactionRepoUpsert(txWithReceipt);
            SetupAddressUpsert(txWithReceipt, tx.From, tx.To, LogAddress1, LogAddress2);

            await _handler.HandleTransactionAsync(txWithReceipt);

            Assert.Equal(1, _txRepoUpsertCount);
            Assert.Equal(4, _addressRepoUpsertCount);
        }

        [Fact]
        public async Task HandleContractCreationTransactionAsync_Calls_Tx_Repo_Once_And_Address_Repo_For_Each_Associated_Address()
        {
            const string ContractAddress = "0x8009b29f2094457d3dba62d1953efea58176ba27";
            const string Code = "bytecode";

            var tx = CreateTransaction();
            var receipt = CreateReceipt(ContractAddress);
            var txWithReceipt = CreateContractCreationTransaction(
                ContractAddress, Code, tx, receipt);

            SetUpTransactionRepoUpsert(txWithReceipt);
            SetupAddressUpsert(txWithReceipt, tx.From, tx.To, LogAddress1, LogAddress2, ContractAddress);

            await _handler.HandleContractCreationTransactionAsync(txWithReceipt);

            Assert.Equal(1, _txRepoUpsertCount);
            Assert.Equal(5, _addressRepoUpsertCount);
        }

        private static TransactionWithReceipt CreateTransactionWithReceipt(Transaction tx, TransactionReceipt receipt)
        {
            return new TransactionWithReceipt(
                tx, receipt, false,
                new HexBigInteger(0), null, false);
        }

        private static TransactionReceipt CreateReceipt()
        {
            return new TransactionReceipt
            {
                Logs = new JArray(
                    JObject.FromObject(new { address = LogAddress1 }),
                    JObject.FromObject(new { address = LogAddress2 })
                ),
            };
        }

        private static Transaction CreateTransaction()
        {
            return new Transaction
            {
                From = "0x1209b29f2094457d3dba62d1953efea58176ba27",
                To = "0x2209b29f2094457d3dba62d1953efea58176ba27"
            };
        }

        private static ContractCreationTransaction CreateContractCreationTransaction(string ContractAddress, string Code, Transaction tx, TransactionReceipt receipt)
        {
            return new ContractCreationTransaction(
                ContractAddress, Code, tx, receipt, false, new HexBigInteger(0));
        }

        private static TransactionReceipt CreateReceipt(string ContractAddress)
        {
            return new TransactionReceipt
            {
                Logs = new JArray(
                    JObject.FromObject(new { address = LogAddress1 }),
                    JObject.FromObject(new { address = LogAddress2 })
                ),
                ContractAddress = ContractAddress
            };
        }

        private void SetupAddressUpsert(TransactionWithReceipt txWithReceipt, params string[] expectedAddresses)
        {
            foreach (var address in expectedAddresses)
            {
                _addressTransactionRepository
                    .Setup(t => t.UpsertAsync(txWithReceipt.Transaction, txWithReceipt.TransactionReceipt,
                        txWithReceipt.HasError, txWithReceipt.BlockTimestamp, address, txWithReceipt.Error,
                        txWithReceipt.HasVmStack, txWithReceipt.TransactionReceipt.ContractAddress))
                    .Returns(() =>
                    {
                        _addressRepoUpsertCount++;
                        return Task.CompletedTask;
                    });
            }
        }

        private void SetUpTransactionRepoUpsert(TransactionWithReceipt txWithReceipt)
        {
            _transactionRepository
                .Setup(t => t.UpsertAsync(
                    txWithReceipt.Transaction,
                    txWithReceipt.TransactionReceipt,
                    txWithReceipt.HasError,
                    txWithReceipt.BlockTimestamp,
                    txWithReceipt.HasVmStack,
                    txWithReceipt.Error))
                .Returns(() =>
                {
                    _txRepoUpsertCount++;
                    return Task.CompletedTask;
                });
        }

        private void SetUpTransactionRepoUpsert(ContractCreationTransaction txWrapper)
        {
            _transactionRepository
                .Setup(t => t.UpsertAsync(
                    txWrapper.ContractAddress,
                    txWrapper.Code,
                    txWrapper.Transaction,
                    txWrapper.TransactionReceipt,
                    txWrapper.FailedCreatingContract,
                    txWrapper.BlockTimestamp))
                .Returns(() =>
                {
                    _txRepoUpsertCount++;
                    return Task.CompletedTask;
                });
        }

        private void SetupAddressUpsert(ContractCreationTransaction txWrapper, params string[] expectedAddresses)
        {
            foreach (var address in expectedAddresses)
            {
                _addressTransactionRepository
                    .Setup(t => t.UpsertAsync(
                        txWrapper.Transaction, 
                        txWrapper.TransactionReceipt,
                        txWrapper.FailedCreatingContract, 
                        txWrapper.BlockTimestamp, 
                        address, 
                        null, 
                        false, 
                        txWrapper.ContractAddress))
                    .Returns(() =>
                    {
                        _addressRepoUpsertCount++;
                        return Task.CompletedTask;
                    });
            }
        }
    }
}
