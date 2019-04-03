using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Handlers.Routers
{
    public class TransactionLogRouterTests
    {
        public class TestEventDto{}

        private readonly Mock<ITransactionLogHandler> _mockTransactionLogHandler = new Mock<ITransactionLogHandler>();
        private readonly Mock<ITransactionLogHandler<TestEventDto>> _mockEventHandler = new Mock<ITransactionLogHandler<TestEventDto>>();

        private readonly Mock<TransactionLogWrapper> _mockLog = new Mock<TransactionLogWrapper>();
        private readonly TransactionLogRouter router = new TransactionLogRouter();

        [Fact]
        public async Task Invokes_Handlers_Without_Conditions()
        {
            router.AddHandler(_mockTransactionLogHandler.Object);
            await router.HandleAsync(_mockLog.Object);
            _mockTransactionLogHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }

        [Fact]
        public async Task Invokes_All_Handlers_Meeting_Condition()
        {
            var handler1 = new Mock<ITransactionLogHandler>();
            var handler2 = new Mock<ITransactionLogHandler>();
            var handler3 = new Mock<ITransactionLogHandler>();

            router.AddHandler((log) => true, handler1.Object);
            router.AddHandler((log) => true, handler2.Object);
            router.AddHandler((log) => false, handler3.Object);

            await router.HandleAsync(_mockLog.Object);

            handler1.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
            handler2.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
            handler3.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Does_Not_Invoke_Handlers_With_Conditions_That_Are_Not_Met()
        {
            router.AddHandler((log) => false, _mockTransactionLogHandler.Object);
            await router.HandleAsync(_mockLog.Object);
            _mockTransactionLogHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Does_Not_Invoke_Handlers_With_Async_Conditions_That_Are_Not_Met()
        {
            router.AddHandler((log) => Task.FromResult(false), _mockTransactionLogHandler.Object);
            await router.HandleAsync(_mockLog.Object);
            _mockTransactionLogHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Invokes_Handlers_With_Conditions()
        {
            router.AddHandler((log) => true, _mockTransactionLogHandler.Object);
            await router.HandleAsync(_mockLog.Object);
            _mockTransactionLogHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }

        [Fact]
        public async Task Invokes_Handlers_With_Async_Conditions()
        {
            router.AddHandler((log) => Task.FromResult(true), _mockTransactionLogHandler.Object);
            await router.HandleAsync(_mockLog.Object);
            _mockTransactionLogHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }

        [Fact]
        public async Task Invokes_Event_Specific_Handlers_Without_Conditions_When_Log_Is_For_Event()
        {
            router.AddHandler(_mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(true);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }

        [Fact]
        public async Task Does_Not_Invoke_Event_Specific_Handlers_When_Log_Is_Not_For_Event()
        {
            router.AddHandler(_mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(false);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Does_Not_Invoke_Event_Specific_Handlers_With_Conditions_That_Are_Not_Met()
        {
            router.AddHandler((log) => false, _mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(true);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Does_Not_Invoke_Event_Specific_Handlers_With_Async_Conditions_That_Are_Not_Met()
        {
            router.AddHandler((log) => Task.FromResult(false), _mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(true);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Never);
        }

        [Fact]
        public async Task Invokes_Event_Specific_Handlers_With_Conditions()
        {
            router.AddHandler((log) => true, _mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(true);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }

        [Fact]
        public async Task Invokes_Event_Specific_Handlers_With_Async_Conditions()
        {
            router.AddHandler((log) => Task.FromResult(true), _mockEventHandler.Object);

            _mockLog.Setup(l => l.IsForEvent<TestEventDto>()).Returns(true);

            await router.HandleAsync(_mockLog.Object);
            _mockEventHandler.Verify(h => h.HandleAsync(_mockLog.Object), Times.Once);
        }
    }
}
