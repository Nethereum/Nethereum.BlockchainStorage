using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers
{
    public class EventHandlerFactoryTests
    {
        EventHandlerFactory _eventHandlerFactory;

        Mock<IEventSubscriptionStateFactory> _stateFactory = new Mock<IEventSubscriptionStateFactory>();
        Mock<IEventContractQueryConfigurationFactory> _contractQueryFactory = new Mock<IEventContractQueryConfigurationFactory>();
        Mock<IContractQuery> _contractQueryHandler = new Mock<IContractQuery>();
        Mock<IEventAggregatorConfigurationFactory> _eventAggregatorConfigurationFactory = new Mock<IEventAggregatorConfigurationFactory>();
        Mock<IGetTransactionByHash> _getTransactionProxy = new Mock<IGetTransactionByHash>();
        Mock<ISubscriberQueueFactory> _subscriberQueueFactory = new Mock<ISubscriberQueueFactory>();
        Mock<ISubscriberSearchIndexFactory> _subscriberSearchIndexFactory = new Mock<ISubscriberSearchIndexFactory>();
        Mock<IEventRuleConfigurationFactory> _eventRuleConfigurationFactory = new Mock<IEventRuleConfigurationFactory>();

        EventSubscriptionStateDto _eventSubscriptionStateDto = new EventSubscriptionStateDto();

        public EventHandlerFactoryTests()
        {
            _eventHandlerFactory = new EventHandlerFactory(
                _stateFactory.Object, 
                _contractQueryFactory.Object, 
                _contractQueryHandler.Object, 
                _eventAggregatorConfigurationFactory.Object, 
                _getTransactionProxy.Object, 
                _subscriberQueueFactory.Object, 
                _subscriberSearchIndexFactory.Object,
                _eventRuleConfigurationFactory.Object);
        }

        [Fact]
        public async Task EventRule()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.Rule
            };

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var eventRuleHandler = handler as EventRule;
            Assert.NotNull(eventRuleHandler);
            Assert.Equal(config.Id, eventRuleHandler.Id);
            Assert.Same(_eventSubscriptionStateDto, eventRuleHandler.State);
        }

        [Fact]
        public async Task Aggregate()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.Aggregate
            };

            var aggregateConfig = new EventAggregatorConfiguration();                

            _eventAggregatorConfigurationFactory
                .Setup(f => f.GetEventAggregationConfigurationAsync(config.Id))
                .ReturnsAsync(aggregateConfig);

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var aggregator = handler as EventAggregator;
            Assert.NotNull(aggregator);
            Assert.Equal(config.Id, aggregator.Id);
            Assert.Same(_eventSubscriptionStateDto, aggregator.State);
            Assert.Same(aggregateConfig, aggregator.Configuration);
        }

        [Fact]
        public async Task ContractQuery()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.ContractQuery
            };

            var contractQueryConfig = new ContractQueryConfiguration();

            _contractQueryFactory
                .Setup(f => f.GetContractQueryConfigurationAsync(config.Id))
                .ReturnsAsync(contractQueryConfig);

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var contractQueryEventHandler = handler as ContractQueryEventHandler;
            Assert.NotNull(contractQueryEventHandler);
            Assert.Equal(config.Id, contractQueryEventHandler.Id);
            Assert.Same(_eventSubscriptionStateDto, contractQueryEventHandler.State);
            Assert.Same(contractQueryConfig, contractQueryEventHandler.Configuration);
        }

        [Fact]
        public async Task Queue()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.Queue,
                SubscriberQueueId = 33
            };

            var queue = new Mock<IQueue>();

            _subscriberQueueFactory
                .Setup(f => f.GetSubscriberQueueAsync(config.SubscriberQueueId))
                .ReturnsAsync(queue.Object);

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var queueHandler = handler as QueueHandler;
            Assert.NotNull(queueHandler);
            Assert.Equal(config.Id, queueHandler.Id);
            Assert.Same(_eventSubscriptionStateDto, queueHandler.State);
            Assert.Same(queue.Object, queueHandler.Queue);
        }

        [Fact]
        public async Task GetTransaction()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.GetTransaction
            };

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var getTransactionHandler = handler as GetTransactionEventHandler;
            Assert.NotNull(getTransactionHandler);
            Assert.Equal(config.Id, getTransactionHandler.Id);
            Assert.Same(_eventSubscriptionStateDto, getTransactionHandler.State);
            Assert.Same(_getTransactionProxy.Object, getTransactionHandler.Proxy);
        }

        [Fact]
        public async Task Index()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.Index
            };

            var searchIndex = new Mock<ISubscriberSearchIndex>();

            _subscriberSearchIndexFactory
                .Setup(f => f.GetSubscriberSearchIndexAsync(config.SubscriberSearchIndexId))
                .ReturnsAsync(searchIndex.Object);

            var handler = await _eventHandlerFactory.LoadAsync(config, _eventSubscriptionStateDto);

            var searchIndexHandler = handler as SearchIndexHandler;
            Assert.NotNull(searchIndexHandler);
            Assert.Equal(config.Id, searchIndexHandler.Id);
            Assert.Same(_eventSubscriptionStateDto, searchIndexHandler.State);
            Assert.Same(searchIndex.Object, searchIndexHandler.SubscriberSearchIndex);
        }
    }
}
