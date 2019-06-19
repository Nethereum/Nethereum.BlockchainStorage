using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockProcessing.Tests.Handlers.Routers
{
    public class TransactionRouterTests
    {
        public class TestDeploymentDto : ContractDeploymentMessage
        {
            public TestDeploymentDto(string byteCode) : base(byteCode)
            {
            }
        }
        public class TestFunctionDto : FunctionMessage{}

        private readonly Mock<ITransactionHandler> _mockTransactionHandler = new Mock<ITransactionHandler>();
        private readonly Mock<ITransactionHandler<TestFunctionDto>> _mockFunctionHandler = new Mock<ITransactionHandler<TestFunctionDto>>();

        private readonly TransactionRouter _router = new TransactionRouter();
        private readonly Mock<TransactionWithReceipt> _transactionWithReceipt = new Mock<TransactionWithReceipt>();

        private readonly Mock<ContractCreationTransaction> _contactCreationTransaction =
            new Mock<ContractCreationTransaction>();

        [Fact]
        public async Task Handlers_Without_Conditions_Are_Invoked()
        {
            _router.AddTransactionHandler(_mockTransactionHandler.Object);
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);
            _mockTransactionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task Handlers_With_Conditions_That_Are_Met_Are_Invoked()
        {
            _router.AddTransactionHandler((txn) => true,  _mockTransactionHandler.Object);
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);
            _mockTransactionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task TransactionsHandled_Represents_Number_Of_Handlers_Actually_Invoked()
        {
            //handlers which will meet condition
            _router.AddTransactionHandler((txn) => true,  new Mock<ITransactionHandler>().Object);
            _router.AddTransactionHandler((txn) => true,  new Mock<ITransactionHandler>().Object);
            //handlers which will not
            _router.AddTransactionHandler((txn) => false,  new Mock<ITransactionHandler>().Object);
            _router.AddTransactionHandler((txn) => false,  new Mock<ITransactionHandler>().Object);

            //invoke 3 times
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            Assert.Equal(6, _router.TransactionsHandled);
        }

        [Fact]
        public async Task ContractsCreated_Represents_Number_Of_Handlers_Actually_Invoked()
        {
            //handlers which will meet condition
            _router.AddContractCreationHandler((txn) => true,  new Mock<ITransactionHandler>().Object);
            _router.AddContractCreationHandler((txn) => true,  new Mock<ITransactionHandler>().Object);
            //handlers which will not
            _router.AddContractCreationHandler((txn) => false,  new Mock<ITransactionHandler>().Object);
            _router.AddContractCreationHandler((txn) => false,  new Mock<ITransactionHandler>().Object);

            //invoke 3 times
            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);
            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);
            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);

            Assert.Equal(6, _router.ContractsCreated);
        }

        [Fact]
        public async Task Handlers_With_Async_Conditions_That_Are_Met_Are_Invoked()
        {
            _router.AddTransactionHandler((txn) => Task.FromResult(true),  _mockTransactionHandler.Object);
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);
            _mockTransactionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Transaction_Is_For_Function_The_Function_Handlers_Are_Invoked()
        {
            _router.AddTransactionHandler(_mockFunctionHandler.Object);

            _transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Transaction_Is_Not_For_That_Function_The_Function_Handler_Is_Not_Invoked()
        {
            _router.AddTransactionHandler(_mockFunctionHandler.Object);

            _transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(false);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Never);
            Assert.Equal(0, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Function_Handler_Condition_Is_Met_It_Is_Invoked()
        {
            //add with a condition that always returns true
            _router.AddTransactionHandler((txn) => true, _mockFunctionHandler.Object);

            //ensure function matches tx
            _transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once);

            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Function_Handler_Async_Condition_Is_Met_It_Is_Invoked()
        {
            //add with a condition that always returns true
            _router.AddTransactionHandler((txn) => Task.FromResult(true), _mockFunctionHandler.Object);

            //ensure function matches tx
            _transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once);

            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Function_Handler_Condition_Is_Not_Met_It_Is_Not_Invoked()
        {
            //add with a condition that always returns false
            _router.AddTransactionHandler((txn) => false, _mockFunctionHandler.Object);

            //ensure function matches tx
            _transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            _mockFunctionHandler
                .Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), 
                Times.Never);

            Assert.Equal(0, _router.TransactionsHandled);
        }

        [Fact]
        public async Task Contract_Creation_Handlers_Are_Not_Invoked_When_Handling_Normal_Transactions()
        {
            var mockContactCreationHandler = new Mock<ITransactionHandler>();
            _router.AddContractCreationHandler(mockContactCreationHandler.Object);

            //handle a non contract creation txn
            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            mockContactCreationHandler
                .Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), 
                    Times.Never);

            Assert.Equal(0, _router.ContractsCreated);
        }

        [Fact]
        public async Task Transaction_Handlers_Are_Not_Invoked_When_Handling_Contract_Creation()
        {
            _router.AddTransactionHandler(_mockTransactionHandler.Object);

            //handle a contract creation (not a normal transaction)
            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);

            _mockTransactionHandler
                .Verify(h => h.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object), 
                    Times.Never);
        }

        [Fact]
        public async Task When_All_Matching_Transaction_Handlers_Match_Condition_All_Are_Invoked()
        {
            var handler1 = new Mock<ITransactionHandler>();
            var handler2 = new Mock<ITransactionHandler>();

            _router.AddTransactionHandler(handler1.Object);
            _router.AddTransactionHandler(handler2.Object);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            handler1.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());
            handler2.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once());

            Assert.Equal(2, _router.TransactionsHandled);
        }

        [Fact]
        public async Task Only_Transaction_Handlers_Matching_The_Condition_Are_Invoked()
        {
            var nonMatchingHandler = new Mock<ITransactionHandler>();
            var matchingHandler = new Mock<ITransactionHandler>();

            _router.AddTransactionHandler((txn) => false, nonMatchingHandler.Object);
            _router.AddTransactionHandler((txn) => true, matchingHandler.Object);

            await _router.HandleTransactionAsync(_transactionWithReceipt.Object);

            nonMatchingHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Never);
            matchingHandler.Verify(h => h.HandleTransactionAsync(_transactionWithReceipt.Object), Times.Once);

            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task When_Contract_Creation_Handlers_Match_Condition_They_Are_Invoked()
        {
            var mockContactCreationHandler = new Mock<ITransactionHandler>();
            _router.AddContractCreationHandler((txn) => true, mockContactCreationHandler.Object);

            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);

            mockContactCreationHandler
                .Verify(h => h.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object), Times.Once);

            Assert.Equal(1, _router.ContractsCreated);
        }

        [Fact]
        public async Task When_Contract_Creation_Handlers_Do_Not_Match_Condition_They_Are_Not_Invoked()
        {
            var mockContactCreationHandler = new Mock<ITransactionHandler>();
            _router.AddContractCreationHandler((txn) => false, mockContactCreationHandler.Object);

            await _router.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object);

            mockContactCreationHandler
                .Verify(h => h.HandleContractCreationTransactionAsync(_contactCreationTransaction.Object), Times.Never);

            Assert.Equal(0, _router.ContractsCreated);
        }
}
}
