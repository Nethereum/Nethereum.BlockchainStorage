using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration;
using Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS.Configuration
{
    public class AzureTablesConfigurationRepoFixture: IDisposable
    {
        public readonly EventProcessingCloudTableSetup CloudTableSetup;
        public ISubscriberRepository SubscriberRepo;
        public ISubscriberContractsRepository SubscriberContractsRepo;
        public IEventSubscriptionRepository EventSubscriptionRepo;
        public IEventSubscriptionAddressRepository EventSubscriptionAddressRepository;
        public IEventHandlerRepository EventHandlerRepository;
        public IParameterConditionRepository ParameterConditionRepository;
        public AzureEventProcessingConfigurationRepository ConfigRepo;
        public IEventSubscriptionStateRepository EventSubscriptionStateRepository;
        public IContractQueryRepository ContractQueryRepository;
        public IContractQueryParameterRepository ContractQueryParameterRepository;
        public IEventAggregatorRepository EventAggregatorRepository;

        public AzureTablesConfigurationRepoFixture()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            CloudTableSetup = new EventProcessingCloudTableSetup(azureStorageConnectionString, "test");

            SubscriberRepo = CloudTableSetup.GetSubscriberRepository();
            SubscriberContractsRepo = CloudTableSetup.GetSubscriberContractsRepository();
            EventSubscriptionRepo = CloudTableSetup.GetEventSubscriptionsRepository();
            EventSubscriptionAddressRepository = CloudTableSetup.GetEventSubscriptionAddressesRepository();
            EventHandlerRepository = CloudTableSetup.GetEventHandlerRepository();
            ParameterConditionRepository = CloudTableSetup.GetParameterConditionRepository();
            EventSubscriptionStateRepository = CloudTableSetup.GetEventSubscriptionStateRepository();
            ContractQueryRepository = CloudTableSetup.GetContractQueryRepository();
            ContractQueryParameterRepository = CloudTableSetup.GetContractQueryParameterRepository();
            EventAggregatorRepository = CloudTableSetup.GetEventAggregatorRepository();

            ConfigRepo = new AzureEventProcessingConfigurationRepository(
                SubscriberRepo, 
                SubscriberContractsRepo, 
                EventSubscriptionRepo, 
                EventSubscriptionAddressRepository,
                EventHandlerRepository,
                ParameterConditionRepository,
                EventSubscriptionStateRepository,
                ContractQueryRepository,
                ContractQueryParameterRepository,
                EventAggregatorRepository);
        }

        public void Dispose()
        {
            foreach(var table in CloudTableSetup.GetCachedTables())
            {
                table.DeleteIfExistsAsync().Wait();
            }
        }

    }

    [CollectionDefinition("AzureTablesConfigurationTests")]
    public class AzureTablesConfigurationRepoFixtureCollection : ICollectionFixture<AzureTablesConfigurationRepoFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("AzureTablesConfigurationTests")]
    public class AzureTablesConfigurationRepoTests
    {
        public AzureTablesConfigurationRepoFixture Fixture { get; }

        public AzureTablesConfigurationRepoTests(AzureTablesConfigurationRepoFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task Subscribers()
        {
            var subscriber1 = new SubscriberDto
            {
                Id = 1,
                PartitionId = 1,
                Disabled = false,
                Name = "One"
            };

            var subscriber2 = new SubscriberDto
            {
                Id = 2,
                PartitionId = 1,
                Disabled = false,
                Name = "Two"
            };

            var subscriber3 = new SubscriberDto
            {
                Id = 3,
                PartitionId = 2,
                Disabled = false,
                Name = "One"
            };


            await Fixture.SubscriberRepo.UpsertAsync(subscriber1);
            await Fixture.SubscriberRepo.UpsertAsync(subscriber2);
            await Fixture.SubscriberRepo.UpsertAsync(subscriber3);

            var partition1Subscribers = await Fixture.ConfigRepo.GetSubscribersAsync(partitionId: 1);
            var partition2Subscribers = await Fixture.ConfigRepo.GetSubscribersAsync(partitionId: 2);

            Assert.Equal(2, partition1Subscribers.Length);
            Assert.Single(partition2Subscribers);
        }

        [Fact]
        public async Task SubscriberContracts()
        {
            var contract1 = new SubscriberContractDto
            {
                Id = 1,
                SubscriberId = 99,
                Abi = "abi1",
                Name = "StandardContract"
            };

            var contract2 = new SubscriberContractDto
            {
                Id = 2,
                SubscriberId = 99
            };


            var contract3 = new SubscriberContractDto
            {
                Id = 3,
                SubscriberId = 100
            };


            await Fixture.SubscriberContractsRepo.UpsertAsync(contract1);
            await Fixture.SubscriberContractsRepo.UpsertAsync(contract2);
            await Fixture.SubscriberContractsRepo.UpsertAsync(contract3);

            var contract1FromRepo = await Fixture.ConfigRepo.GetSubscriberContractAsync(subscriberId:99, contractId: 1);
            var contract2FromRepo = await Fixture.ConfigRepo.GetSubscriberContractAsync(subscriberId: 99, contractId: 2);
            var contract3FromRepo = await Fixture.ConfigRepo.GetSubscriberContractAsync(subscriberId: 100, contractId: 3);


            Assert.NotNull(contract1FromRepo);
            Assert.Equal(contract1.Abi, contract1FromRepo.Abi);
            Assert.Equal(contract1.Name, contract1FromRepo.Name);
            Assert.NotNull(contract2FromRepo);
            Assert.NotNull(contract3FromRepo);

        }

        [Fact]
        public async Task EventSubscriptions()
        {
            var sub1 = new EventSubscriptionDto
            {
                Id = 1,
                SubscriberId = 99,
                CatchAllContractEvents = false,
                ContractId = 2,
                Disabled = false,
                EventSignatures = new System.Collections.Generic.List<string>(new[] {"1", "2"})
            };

            var sub2 = new EventSubscriptionDto
            {
                Id = 2,
                SubscriberId = 99,
                CatchAllContractEvents = false,
                ContractId = 2,
                Disabled = false,
                EventSignatures = new System.Collections.Generic.List<string>(new[] { "", "" })
            };

            var sub3 = new EventSubscriptionDto
            {
                Id = 3,
                SubscriberId = 100,
                CatchAllContractEvents = false,
                ContractId = 2,
                Disabled = false,
                EventSignatures = new System.Collections.Generic.List<string>(new[] { "", "" })
            };

            await Fixture.EventSubscriptionRepo.UpsertAsync(sub1);
            await Fixture.EventSubscriptionRepo.UpsertAsync(sub2);
            await Fixture.EventSubscriptionRepo.UpsertAsync(sub3);

            var sub1Subscriptions = await Fixture.ConfigRepo.GetEventSubscriptionsAsync(99);
            var sub2Subscriptions = await Fixture.ConfigRepo.GetEventSubscriptionsAsync(100);

            Assert.Equal(2, sub1Subscriptions.Length);
            Assert.Single(sub2Subscriptions);

            var sub1FromRepo = sub1Subscriptions.First();
            Assert.Equal(sub1.CatchAllContractEvents, sub1FromRepo.CatchAllContractEvents);
            Assert.Equal(sub1.ContractId, sub1FromRepo.ContractId);
            Assert.Equal(sub1.Disabled, sub1FromRepo.Disabled);
            Assert.Equal(sub1.EventSignatures, sub1FromRepo.EventSignatures);
        }

        [Fact]
        public async Task EventSubscriptionAddresses()
        {
            var addr1 = new EventSubscriptionAddressDto {Id = 1, EventSubscriptionId = 99, Address = "xyz" };
            var addr2 = new EventSubscriptionAddressDto { Id = 2, EventSubscriptionId = 99, Address = "abc" };
            var addr3 = new EventSubscriptionAddressDto { Id = 3, EventSubscriptionId = 100, Address = "qer" };

            await Fixture.EventSubscriptionAddressRepository.UpsertAsync(addr1);
            await Fixture.EventSubscriptionAddressRepository.UpsertAsync(addr2);
            await Fixture.EventSubscriptionAddressRepository.UpsertAsync(addr3);

            var sub1Addresses = await Fixture.ConfigRepo.GetEventSubscriptionAddressesAsync(99);
            var sub2Addresses = await Fixture.ConfigRepo.GetEventSubscriptionAddressesAsync(100);

            Assert.Equal(2, sub1Addresses.Length);
            Assert.Single(sub2Addresses);

            Assert.Equal(addr1.Address, sub1Addresses[0].Address);

        }

        [Fact]
        public async Task EventHandlers()
        {
            var handler1 = new EventHandlerDto { 
                Id = 1, 
                EventSubscriptionId = 99, 
                Disabled = false, 
                HandlerType = 
                Processing.Logs.Handling.EventHandlerType.Queue, 
                Order = 1, 
                SubscriberQueueId = 5
            };

            var handler2 = new EventHandlerDto
            {
                Id = 2,
                EventSubscriptionId = 99,
                Disabled = false,
                HandlerType =
                Processing.Logs.Handling.EventHandlerType.Index,
                Order = 2,
                SubscriberSearchIndexId = 5
            };

            var handler3 = new EventHandlerDto
            {
                Id = 3,
                EventSubscriptionId = 100,
                Disabled = false,
                HandlerType =
                Processing.Logs.Handling.EventHandlerType.Store,
                Order = 1,
                SubscriberRepositoryId = 7
            };

            await Fixture.EventHandlerRepository.UpsertAsync(handler1);
            await Fixture.EventHandlerRepository.UpsertAsync(handler2);
            await Fixture.EventHandlerRepository.UpsertAsync(handler3);

            var sub1Handlers = await Fixture.ConfigRepo.GetEventHandlersAsync(99);
            var sub2Handlers = await Fixture.ConfigRepo.GetEventHandlersAsync(100);

            Assert.Equal(2, sub1Handlers.Length);
            Assert.Single(sub2Handlers);

            var queueHandler = sub1Handlers[0];
            Assert.Equal(handler1.Disabled, queueHandler.Disabled);
            Assert.Equal(handler1.HandlerType, queueHandler.HandlerType);
            Assert.Equal(handler1.Order, queueHandler.Order);
            Assert.Equal(handler1.SubscriberQueueId, queueHandler.SubscriberQueueId);

            var searchIndexHandler = sub1Handlers[1];
            Assert.Equal(handler2.SubscriberSearchIndexId, searchIndexHandler.SubscriberSearchIndexId);

            var storageHandler = sub2Handlers[0];
            Assert.Equal(handler3.SubscriberRepositoryId, storageHandler.SubscriberRepositoryId);
        }

        [Fact]
        public async Task ParameterConditions()
        {
            var condition1 = new ParameterConditionDto { Id = 1, EventSubscriptionId = 99, Operator = ParameterConditionOperator.GreaterOrEqual, ParameterOrder = 1, Value = "10"};
            var condition2 = new ParameterConditionDto { Id = 2, EventSubscriptionId = 99, Operator = ParameterConditionOperator.LessOrEqual, ParameterOrder = 2, Value = "5" };
            var condition3 = new ParameterConditionDto { Id = 3, EventSubscriptionId = 100, Operator = ParameterConditionOperator.Equals, ParameterOrder = 1, Value = "11" };

            await Fixture.ParameterConditionRepository.UpsertAsync(condition1);
            await Fixture.ParameterConditionRepository.UpsertAsync(condition2);
            await Fixture.ParameterConditionRepository.UpsertAsync(condition3);

            var sub1Conditions = await Fixture.ConfigRepo.GetParameterConditionsAsync(99);
            var sub2Conditions = await Fixture.ConfigRepo.GetParameterConditionsAsync(100);

            Assert.Equal(2, sub1Conditions.Length);
            Assert.Single(sub2Conditions);

            var c1 = sub1Conditions[0];
            Assert.Equal(condition1.Operator, c1.Operator);
            Assert.Equal(condition1.ParameterOrder, c1.ParameterOrder);
            Assert.Equal(condition1.Value, c1.Value);

           }

        [Fact]
        public async Task EventSubscriptionState()
        {
            //initialising state for a new sub
            var newState = await Fixture.ConfigRepo.GetOrCreateEventSubscriptionStateAsync(100);
            Assert.Equal((long)100, newState.Id);
            Assert.Equal((long)100, newState.EventSubscriptionId);
            Assert.NotNull(newState.Values);

            //getting state for existing sub
            var stateDto = new EventSubscriptionStateDto
            {
                EventSubscriptionId = 99, Values = new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "key1", (object)"val1" },
                    { "key2", (object)1 }
                }
            };

            await Fixture.ConfigRepo.UpsertAsync(new[] { stateDto});

            var fromRepo = await Fixture.ConfigRepo.GetOrCreateEventSubscriptionStateAsync(99);

            Assert.Equal(99, fromRepo.EventSubscriptionId);
            Assert.Equal(99, fromRepo.Id);
            Assert.Equal("val1", fromRepo.Values["key1"].ToString());
            Assert.Equal((long)1, fromRepo.Values["key2"]);
        }

        [Fact]
        public async Task ContractQueries()
        {
            var contractDto = new SubscriberContractDto
            {
                SubscriberId = 999,
                Id = 1001,
                Abi = "{}",
                Name = "StandardContract"
            };

            //dummy values - purely to ensure the repo returns all expected values
            //not meant to be actually consistent with a typical record 
            var contractQuery = new ContractQueryDto
            {
                ContractId = contractDto.Id,
                ContractAddress = "ContractAddress",
                ContractAddressParameterNumber = 2,
                ContractAddressSource = ContractAddressSource.EventParameter,
                ContractAddressStateVariableName = "ContractAddressStateVariableName",
                EventHandlerId = 200,
                EventStateOutputName = "EventStateOutputName",
                FunctionSignature = "FunctionSignature",
                SubscriptionStateOutputName = "SubscriptionStateOutputName"
            };

            var queryParam1 = new ContractQueryParameterDto
            {
                ContractQueryId = contractQuery.EventHandlerId,
                EventParameterNumber = 1,
                Order = 1,
                EventStateName = "EventStateName",
                Id = 567,
                Source = EventValueSource.EventParameters,
                Value = "Value"
            };

            var queryParam2 = new ContractQueryParameterDto
            {
                ContractQueryId = contractQuery.EventHandlerId,
                EventParameterNumber = 2,
                Order = 2,
                EventStateName = "EventStateName",
                Id = 568,
                Source = EventValueSource.EventParameters,
                Value = "Value"
            };

            var queryParameterDtos = new[] { queryParam1, queryParam2 };

            await Fixture.SubscriberContractsRepo.UpsertAsync(contractDto);
            await Fixture.ContractQueryRepository.UpsertAsync(contractQuery);
            await Fixture.ContractQueryParameterRepository.UpsertAsync(queryParam1);
            await Fixture.ContractQueryParameterRepository.UpsertAsync(queryParam2);

            var configFromRepo = await Fixture.ConfigRepo.GetContractQueryConfigurationAsync(contractDto.SubscriberId, contractQuery.EventHandlerId);

            Assert.Equal(contractDto.Abi, configFromRepo.ContractABI);
            Assert.Equal(contractQuery.ContractAddress, configFromRepo.ContractAddress);
            Assert.Equal(contractQuery.ContractAddressParameterNumber, configFromRepo.ContractAddressParameterNumber);
            Assert.Equal(contractQuery.ContractAddressSource, configFromRepo.ContractAddressSource);
            Assert.Equal(contractQuery.ContractAddressStateVariableName, configFromRepo.ContractAddressStateVariableName);
            Assert.Equal(contractQuery.EventStateOutputName, configFromRepo.EventStateOutputName);
            Assert.Equal(contractQuery.FunctionSignature, configFromRepo.FunctionSignature);
            Assert.Equal(contractQuery.SubscriptionStateOutputName, configFromRepo.SubscriptionStateOutputName);

            Assert.Equal(queryParameterDtos.Length, configFromRepo.Parameters.Length);

            for(var i = 0; i < queryParameterDtos.Length; i++)
            {
                var dto = queryParameterDtos[i];
                var actual = configFromRepo.Parameters[i];

                Assert.Equal(dto.EventParameterNumber, actual.EventParameterNumber);
                Assert.Equal(dto.EventStateName, actual.EventStateName);
                Assert.Equal(dto.Order, actual.Order);
                Assert.Equal(dto.Source, actual.Source);
                Assert.Equal(dto.Value, actual.Value);
            }
        }

        [Fact]
        public async Task EventAggregators()
        {
            var aggregatorDto = new EventAggregatorDto { 
                EventHandlerId = 800, 
                Destination = AggregatorDestination.EventSubscriptionState,
                EventParameterNumber = 2,
                Operation = AggregatorOperation.Sum,
                OutputKey = "SumOfSomething",
                Source = AggregatorSource.EventParameter,
                SourceKey = "Value"};

            await Fixture.EventAggregatorRepository.UpsertAsync(aggregatorDto);

            var fromRepo = await Fixture.ConfigRepo.GetEventAggregationConfigurationAsync(aggregatorDto.EventHandlerId);

            Assert.Equal(aggregatorDto.Destination, fromRepo.Destination);
            Assert.Equal(aggregatorDto.EventParameterNumber, fromRepo.EventParameterNumber);
            Assert.Equal(aggregatorDto.Operation, fromRepo.Operation);
            Assert.Equal(aggregatorDto.OutputKey, fromRepo.OutputKey);
            Assert.Equal(aggregatorDto.Source, fromRepo.Source);
            Assert.Equal(aggregatorDto.SourceKey, fromRepo.SourceKey);
        }

    }
}
