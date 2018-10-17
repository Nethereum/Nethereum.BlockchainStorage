using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Handlers.Routers
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
        public class TestEventDto{}

        private readonly Mock<ITransactionHandler> _mockTransactionHandler = new Mock<ITransactionHandler>();
        private readonly Mock<ITransactionHandler<TestFunctionDto>> _mockFunctionHandler = new Mock<ITransactionHandler<TestFunctionDto>>();

        private readonly TransactionRouter _router = new TransactionRouter();
        private readonly Mock<TransactionWithReceipt> transactionWithReceipt = new Mock<TransactionWithReceipt>();

        [Fact]
        public async Task RoutesTransactionsToHandlersAddedWithoutConditions()
        {
            _router.AddTransactionHandler(_mockTransactionHandler.Object);
            await _router.HandleTransactionAsync(transactionWithReceipt.Object);
            _mockTransactionHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task OnlyRoutesToFunctionHandlersIfTxnIsForThatFunction()
        {
            _router.AddFunctionHandler(_mockFunctionHandler.Object);

            transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Once());
            Assert.Equal(1, _router.TransactionsHandled);
        }

        [Fact]
        public async Task DoesNotRouteToFunctionHandlerIfTxnIsNotForThatFunction()
        {
            _router.AddFunctionHandler(_mockFunctionHandler.Object);

            transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(false);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Never);
            Assert.Equal(0, _router.TransactionsHandled);
        }

        [Fact]
        public async Task DoesNotRouteToFunctionHandlerIfConditionIsNotMet()
        {
            //add with a condition that always returns false
            _router.AddFunctionHandler((txn) => false, _mockFunctionHandler.Object);

            //ensure function matches tx
            transactionWithReceipt.Setup(t => t.IsForFunction<TestFunctionDto>()).Returns(true);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            _mockFunctionHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Never);

            Assert.Equal(0, _router.TransactionsHandled);
        }

        [Fact]
        public async Task RoutesASingleTransactionToAllHandlersMatchingCondition()
        {
            var handler1 = new Mock<ITransactionHandler>();
            var handler2 = new Mock<ITransactionHandler>();

            _router.AddTransactionHandler(handler1.Object);
            _router.AddTransactionHandler(handler2.Object);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            handler1.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Once());
            handler2.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Once());

            Assert.Equal(2, _router.TransactionsHandled);
        }

        [Fact]
        public async Task WhenConditionIsNotMetDoesNotRouteToTheHandler()
        {
            var handler = new Mock<ITransactionHandler>();
            _router.AddTransactionHandler((txn) => false, handler.Object);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            handler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Never);
            Assert.Equal(0, _router.TransactionsHandled);
        }

        [Fact]
        public async Task WhenThereAreManyHandlersWillOnlyRouteToThoseMatchingCondition()
        {
            var nonMatchingHandler = new Mock<ITransactionHandler>();
            var matchingHandler = new Mock<ITransactionHandler>();

            _router.AddTransactionHandler((txn) => false, nonMatchingHandler.Object);
            _router.AddTransactionHandler((txn) => true, matchingHandler.Object);

            await _router.HandleTransactionAsync(transactionWithReceipt.Object);

            nonMatchingHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Never);
            matchingHandler.Verify(h => h.HandleTransactionAsync(transactionWithReceipt.Object), Times.Once);

            Assert.Equal(1, _router.TransactionsHandled);
        }
}
}
