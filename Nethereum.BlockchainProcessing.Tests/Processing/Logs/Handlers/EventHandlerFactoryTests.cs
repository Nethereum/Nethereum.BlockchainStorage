using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers
{
    public class EventHandlerFactoryTests
    {
        EventHandlerFactory _eventHandlerFactory;

        Web3Mock web3Mock = new Web3Mock();

        Mock<IEventSubscriptionStateRepository> _stateFactory = new Mock<IEventSubscriptionStateRepository>();
        Mock<IEventContractQueryConfigurationRepository> _contractQueryFactory = new Mock<IEventContractQueryConfigurationRepository>();
        Mock<IContractQuery> _contractQueryHandler = new Mock<IContractQuery>();
        Mock<IEventAggregatorRepository> _eventAggregatorConfigurationFactory = new Mock<IEventAggregatorRepository>();
        Mock<ISubscriberQueueFactory> _subscriberQueueFactory = new Mock<ISubscriberQueueFactory>();
        Mock<ISubscriberSearchIndexFactory> _subscriberSearchIndexFactory = new Mock<BlockchainProcessing.Processing.Logs.Handling.ISubscriberSearchIndexFactory>();
        Mock<IEventRuleRepository> _eventRuleRepository = new Mock<IEventRuleRepository>();
        Mock<ISubscriberStorageFactory> _subscriberStorageFactory = new Mock<ISubscriberStorageFactory>();

        Mock<ISubscriberQueueRepository> _subscriberQueueRepository = new Mock<ISubscriberQueueRepository>();
        Mock<ISubscriberSearchIndexRepository> _subscriberSearchIndexRepository = new Mock<ISubscriberSearchIndexRepository>();
        Mock<ISubscriberStorageRepository> _subscriberStorageRepository = new Mock<ISubscriberStorageRepository>();

        EventSubscriptionStateDto _eventSubscriptionStateDto = new EventSubscriptionStateDto();
        Mock<IEventSubscription> _mockEventSubscription;

        public EventHandlerFactoryTests()
        {
            _eventHandlerFactory = new EventHandlerFactory(
                _stateFactory.Object, 
                _contractQueryFactory.Object, 
                _contractQueryHandler.Object, 
                _eventAggregatorConfigurationFactory.Object, 
                web3Mock.GetTransactionByHashMock.Object,
                _subscriberQueueRepository.Object,
                _subscriberQueueFactory.Object,
                _subscriberSearchIndexRepository.Object,
                _subscriberSearchIndexFactory.Object,
                _eventRuleRepository.Object,
                _subscriberStorageRepository.Object,
                _subscriberStorageFactory.Object);

            _mockEventSubscription = new Mock<IEventSubscription>();
            _mockEventSubscription.Setup(s => s.State).Returns(_eventSubscriptionStateDto);
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

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var eventRuleHandler = handler as EventRule;
            Assert.NotNull(eventRuleHandler);
            Assert.Equal(config.Id, eventRuleHandler.Id);
            Assert.Same(_mockEventSubscription.Object, eventRuleHandler.Subscription);
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

            var aggregateConfig = new EventAggregatorDto();                

            _eventAggregatorConfigurationFactory
                .Setup(f => f.GetAsync(config.Id))
                .ReturnsAsync(aggregateConfig);

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var aggregator = handler as EventAggregator;
            Assert.NotNull(aggregator);
            Assert.Equal(config.Id, aggregator.Id);
            Assert.Same(_mockEventSubscription.Object, aggregator.Subscription);
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
                .Setup(f => f.GetAsync(_mockEventSubscription.Object.SubscriberId, config.Id))
                .ReturnsAsync(contractQueryConfig);

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var contractQueryEventHandler = handler as ContractQueryEventHandler;
            Assert.NotNull(contractQueryEventHandler);
            Assert.Equal(config.Id, contractQueryEventHandler.Id);
            Assert.Same(_mockEventSubscription.Object, contractQueryEventHandler.Subscription);
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

            var queueConfig = new SubscriberQueueDto();
            var queue = new Mock<IQueue>();

            _subscriberQueueRepository
                .Setup(r => r.GetAsync(_mockEventSubscription.Object.Id, config.SubscriberQueueId))
                .ReturnsAsync(queueConfig);

            _subscriberQueueFactory
                .Setup(f => f.GetSubscriberQueueAsync(queueConfig))
                .ReturnsAsync(queue.Object);

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var queueHandler = handler as QueueHandler;
            Assert.NotNull(queueHandler);
            Assert.Equal(config.Id, queueHandler.Id);
            Assert.Same(_mockEventSubscription.Object, queueHandler.Subscription);
            Assert.Same(queue.Object, queueHandler.Queue);
        }

        [Fact]
        public async Task Store()
        {
            var config = new EventHandlerDto
            {
                Id = 50,
                EventSubscriptionId = 99,
                HandlerType = EventHandlerType.Store,
                SubscriberQueueId = 33
            };

            var storageConfig = new SubscriberStorageDto();
            var repo = new Mock<ILogHandler>();

            _subscriberStorageRepository
                .Setup(r => r.GetAsync(_mockEventSubscription.Object.SubscriberId, config.SubscriberRepositoryId))
                .ReturnsAsync(storageConfig);

            _subscriberStorageFactory
                .Setup(f => f.GetLogRepositoryHandlerAsync(storageConfig))
                .ReturnsAsync(repo.Object);

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var repositoryHandler = handler as StorageHandler;
            Assert.NotNull(repositoryHandler);
            Assert.Equal(config.Id, repositoryHandler.Id);
            Assert.Same(_mockEventSubscription.Object, repositoryHandler.Subscription);
            Assert.Same(repo.Object, repositoryHandler.LogHandler);
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

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var getTransactionHandler = handler as GetTransactionEventHandler;
            Assert.NotNull(getTransactionHandler);
            Assert.Equal(config.Id, getTransactionHandler.Id);
            Assert.Same(_mockEventSubscription.Object, getTransactionHandler.Subscription);
            Assert.Same(web3Mock.GetTransactionByHashMock.Object, getTransactionHandler.GetTransactionProxy);
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

            var searchIndexConfig = new SubscriberSearchIndexDto();
            var searchIndex = new Mock<ISubscriberSearchIndex>();

            _subscriberSearchIndexRepository
                .Setup(r => r.GetAsync(_mockEventSubscription.Object.SubscriberId, config.SubscriberSearchIndexId))
                .ReturnsAsync(searchIndexConfig);
                

            _subscriberSearchIndexFactory
                .Setup(f => f.GetSubscriberSearchIndexAsync(searchIndexConfig))
                .ReturnsAsync(searchIndex.Object);

            var handler = await _eventHandlerFactory.LoadAsync(_mockEventSubscription.Object, config);

            var searchIndexHandler = handler as SearchIndexHandler;
            Assert.NotNull(searchIndexHandler);
            Assert.Equal(config.Id, searchIndexHandler.Id);
            Assert.Same(_mockEventSubscription.Object, searchIndexHandler.Subscription);
            Assert.Same(searchIndex.Object, searchIndexHandler.SubscriberSearchIndex);
        }
    }
}
