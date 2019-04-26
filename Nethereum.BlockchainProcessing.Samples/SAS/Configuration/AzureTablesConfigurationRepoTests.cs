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
        private readonly EventProcessingCloudTableSetup CloudTableSetup;
        public readonly IEventProcessingConfigurationRepository ConfigRepo;

        public AzureTablesConfigurationRepoFixture()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            CloudTableSetup = new EventProcessingCloudTableSetup(azureStorageConnectionString, "test");
            ConfigRepo = new AzureEventProcessingConfigurationRepository(CloudTableSetup);
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


            await Fixture.ConfigRepo.Subscribers.UpsertAsync(subscriber1);
            await Fixture.ConfigRepo.Subscribers.UpsertAsync(subscriber2);
            await Fixture.ConfigRepo.Subscribers.UpsertAsync(subscriber3);

            var partition1Subscribers = await Fixture.ConfigRepo.Subscribers.GetManyAsync(partitionId: 1);
            var partition2Subscribers = await Fixture.ConfigRepo.Subscribers.GetManyAsync(partitionId: 2);

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


            await Fixture.ConfigRepo.SubscriberContracts.UpsertAsync(contract1);
            await Fixture.ConfigRepo.SubscriberContracts.UpsertAsync(contract2);
            await Fixture.ConfigRepo.SubscriberContracts.UpsertAsync(contract3);

            var contract1FromRepo = await Fixture.ConfigRepo.SubscriberContracts.GetAsync(subscriberId:99, id: 1);
            var contract2FromRepo = await Fixture.ConfigRepo.SubscriberContracts.GetAsync(subscriberId: 99, id: 2);
            var contract3FromRepo = await Fixture.ConfigRepo.SubscriberContracts.GetAsync(subscriberId: 100, id: 3);


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

            await Fixture.ConfigRepo.EventSubscriptions.UpsertAsync(sub1);
            await Fixture.ConfigRepo.EventSubscriptions.UpsertAsync(sub2);
            await Fixture.ConfigRepo.EventSubscriptions.UpsertAsync(sub3);

            var sub1Subscriptions = await Fixture.ConfigRepo.EventSubscriptions.GetManyAsync(99);
            var sub2Subscriptions = await Fixture.ConfigRepo.EventSubscriptions.GetManyAsync(100);

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

            await Fixture.ConfigRepo.EventSubscriptionAddresses.UpsertAsync(addr1);
            await Fixture.ConfigRepo.EventSubscriptionAddresses.UpsertAsync(addr2);
            await Fixture.ConfigRepo.EventSubscriptionAddresses.UpsertAsync(addr3);

            var sub1Addresses = await Fixture.ConfigRepo.EventSubscriptionAddresses.GetManyAsync(99);
            var sub2Addresses = await Fixture.ConfigRepo.EventSubscriptionAddresses.GetManyAsync(100);

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

            await Fixture.ConfigRepo.EventHandlers.UpsertAsync(handler1);
            await Fixture.ConfigRepo.EventHandlers.UpsertAsync(handler2);
            await Fixture.ConfigRepo.EventHandlers.UpsertAsync(handler3);

            var sub1Handlers = await Fixture.ConfigRepo.EventHandlers.GetManyAsync(99);
            var sub2Handlers = await Fixture.ConfigRepo.EventHandlers.GetManyAsync(100);

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

            await Fixture.ConfigRepo.ParameterConditions.UpsertAsync(condition1);
            await Fixture.ConfigRepo.ParameterConditions.UpsertAsync(condition2);
            await Fixture.ConfigRepo.ParameterConditions.UpsertAsync(condition3);

            var sub1Conditions = await Fixture.ConfigRepo.ParameterConditions.GetManyAsync(99);
            var sub2Conditions = await Fixture.ConfigRepo.ParameterConditions.GetManyAsync(100);

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
            var newState = await Fixture.ConfigRepo.EventSubscriptionStates.GetAsync(100);
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

            await Fixture.ConfigRepo.EventSubscriptionStates.UpsertAsync(new[] { stateDto});

            var fromRepo = await Fixture.ConfigRepo.EventSubscriptionStates.GetAsync(99);

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

            await Fixture.ConfigRepo.SubscriberContracts.UpsertAsync(contractDto);
            await Fixture.ConfigRepo.ContractQueries.UpsertAsync(contractQuery);
            await Fixture.ConfigRepo.ContractQueryParameters.UpsertAsync(queryParam1);
            await Fixture.ConfigRepo.ContractQueryParameters.UpsertAsync(queryParam2);

            var configFromRepo = await Fixture.ConfigRepo.EventContractQueries.GetAsync(contractDto.SubscriberId, contractQuery.EventHandlerId);

            Assert.Equal(contractDto.Abi, configFromRepo.Contract.Abi);
            Assert.Equal(contractQuery.ContractAddress, configFromRepo.Query.ContractAddress);
            Assert.Equal(contractQuery.ContractAddressParameterNumber, configFromRepo.Query.ContractAddressParameterNumber);
            Assert.Equal(contractQuery.ContractAddressSource, configFromRepo.Query.ContractAddressSource);
            Assert.Equal(contractQuery.ContractAddressStateVariableName, configFromRepo.Query.ContractAddressStateVariableName);
            Assert.Equal(contractQuery.EventStateOutputName, configFromRepo.Query.EventStateOutputName);
            Assert.Equal(contractQuery.FunctionSignature, configFromRepo.Query.FunctionSignature);
            Assert.Equal(contractQuery.SubscriptionStateOutputName, configFromRepo.Query.SubscriptionStateOutputName);

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

            await Fixture.ConfigRepo.EventAggregators.UpsertAsync(aggregatorDto);

            var fromRepo = await Fixture.ConfigRepo.EventAggregators.GetAsync(aggregatorDto.EventHandlerId);

            Assert.Equal(aggregatorDto.Destination, fromRepo.Destination);
            Assert.Equal(aggregatorDto.EventParameterNumber, fromRepo.EventParameterNumber);
            Assert.Equal(aggregatorDto.Operation, fromRepo.Operation);
            Assert.Equal(aggregatorDto.OutputKey, fromRepo.OutputKey);
            Assert.Equal(aggregatorDto.Source, fromRepo.Source);
            Assert.Equal(aggregatorDto.SourceKey, fromRepo.SourceKey);
        }

        [Fact]
        public async Task EventRules()
        {
            var dto1 = new EventRuleDto {
                EventHandlerId = 900,
                EventParameterNumber = 1,
                InputName = "From",
                Source = EventRuleSource.EventParameter,
                Type = EventRuleType.GreaterOrEqualTo,
                Value = "10"
                };

            var dto2 = new EventRuleDto
            {
                EventHandlerId = 901,
                InputName = "RunningTotal",
                Source = EventRuleSource.EventState,
                Type = EventRuleType.LessThanOrEqualTo,
                Value = "10"
            };

            await Fixture.ConfigRepo.EventRules.UpsertAsync(dto1);
            await Fixture.ConfigRepo.EventRules.UpsertAsync(dto2);

            var fromRepo1 = await Fixture.ConfigRepo.EventRules.GetAsync(dto1.EventHandlerId);
            var fromRepo2 = await Fixture.ConfigRepo.EventRules.GetAsync(dto2.EventHandlerId);

            Assert.Equal(dto1.EventHandlerId, fromRepo1.EventHandlerId);
            Assert.Equal(dto1.EventParameterNumber, fromRepo1.EventParameterNumber);
            Assert.Equal(dto1.InputName, fromRepo1.InputName);
            Assert.Equal(dto1.Source, fromRepo1.Source);
            Assert.Equal(dto1.Type, fromRepo1.Type);
            Assert.Equal(dto1.Value, fromRepo1.Value);

            Assert.Equal(dto2.EventHandlerId, fromRepo2.EventHandlerId);
        }


        [Fact]
        public async Task SubscriberQueues()
        {
            var queueDto1 = new SubscriberQueueDto {SubscriberId = 99, Id = 1099, Disabled = false, Name = "transfers" };
            var queueDto2 = new SubscriberQueueDto { SubscriberId = 100, Id = 1100, Disabled = false, Name = "approvals" };

            await Fixture.ConfigRepo.SubscriberQueues.UpsertAsync(queueDto1);
            await Fixture.ConfigRepo.SubscriberQueues.UpsertAsync(queueDto2);

            var fromRepo1 = await Fixture.ConfigRepo.SubscriberQueues.GetAsync(queueDto1.SubscriberId, queueDto1.Id);

            Assert.Equal(queueDto1.Id, fromRepo1.Id);
            Assert.Equal(queueDto1.SubscriberId, fromRepo1.SubscriberId);
            Assert.Equal(queueDto1.Disabled, fromRepo1.Disabled);
            Assert.Equal(queueDto1.Name, fromRepo1.Name);

            var fromRepo2 = await Fixture.ConfigRepo.SubscriberQueues.GetAsync(queueDto2.SubscriberId, queueDto2.Id);

            Assert.Equal(queueDto2.Id, fromRepo2.Id);
        }

        [Fact]
        public async Task SubscriberSearchIndexes()
        {
            var dto1 = new SubscriberSearchIndexDto { SubscriberId = 99, Id = 1099, Disabled = false, Name = "transfers" };
            var dto2 = new SubscriberSearchIndexDto { SubscriberId = 100, Id = 1100, Disabled = false, Name = "approvals" };

            await Fixture.ConfigRepo.SubscriberSearchIndexes.UpsertAsync(dto1);
            await Fixture.ConfigRepo.SubscriberSearchIndexes.UpsertAsync(dto2);

            var fromRepo1 = await Fixture.ConfigRepo.SubscriberSearchIndexes.GetAsync(dto1.SubscriberId, dto1.Id);

            Assert.Equal(dto1.Id, fromRepo1.Id);
            Assert.Equal(dto1.SubscriberId, fromRepo1.SubscriberId);
            Assert.Equal(dto1.Disabled, fromRepo1.Disabled);
            Assert.Equal(dto1.Name, fromRepo1.Name);

            var fromRepo2 = await Fixture.ConfigRepo.SubscriberSearchIndexes.GetAsync(dto2.SubscriberId, dto2.Id);

            Assert.Equal(dto2.Id, fromRepo2.Id);
        }

        [Fact]
        public async Task SubscriberStorage()
        {
            var dto1 = new SubscriberStorageDto { SubscriberId = 99, Id = 1099, Disabled = false, Name = "transfers" };
            var dto2 = new SubscriberStorageDto { SubscriberId = 100, Id = 1100, Disabled = false, Name = "approvals" };

            await Fixture.ConfigRepo.SubscriberStorage.UpsertAsync(dto1);
            await Fixture.ConfigRepo.SubscriberStorage.UpsertAsync(dto2);

            var fromRepo1 = await Fixture.ConfigRepo.SubscriberStorage.GetAsync(dto1.SubscriberId, dto1.Id);

            Assert.Equal(dto1.Id, fromRepo1.Id);
            Assert.Equal(dto1.SubscriberId, fromRepo1.SubscriberId);
            Assert.Equal(dto1.Disabled, fromRepo1.Disabled);
            Assert.Equal(dto1.Name, fromRepo1.Name);

            var fromRepo2 = await Fixture.ConfigRepo.SubscriberStorage.GetAsync(dto2.SubscriberId, dto2.Id);

            Assert.Equal(dto2.Id, fromRepo2.Id);
        }

        [Fact]
        public async Task EventHandlerHistory()
        {
            var dto1 = new EventHandlerHistoryDto {SubscriberId = 99, EventSubscriptionId = 101, EventHandlerId = 987, EventKey = "xyz1" };
            var dto2 = new EventHandlerHistoryDto { SubscriberId = 99, EventSubscriptionId = 101, EventHandlerId = 987, EventKey = "xyz2" };
            var dto3 = new EventHandlerHistoryDto { SubscriberId = 100, EventSubscriptionId = 102, EventHandlerId = 988, EventKey = "xyz1" };

            await Fixture.ConfigRepo.EventHandlerHistoryRepo.UpsertAsync(dto1);
            await Fixture.ConfigRepo.EventHandlerHistoryRepo.UpsertAsync(dto2);
            await Fixture.ConfigRepo.EventHandlerHistoryRepo.UpsertAsync(dto3);

            Assert.True(await Fixture.ConfigRepo.EventHandlerHistoryRepo.ContainsAsync(dto1.EventHandlerId, dto1.EventKey));
            Assert.True(await Fixture.ConfigRepo.EventHandlerHistoryRepo.ContainsAsync(dto2.EventHandlerId, dto2.EventKey));
            Assert.True(await Fixture.ConfigRepo.EventHandlerHistoryRepo.ContainsAsync(dto2.EventHandlerId, dto2.EventKey));

            Assert.False(await Fixture.ConfigRepo.EventHandlerHistoryRepo.ContainsAsync(1232345346, "blahblah"));

            var fromRep = await Fixture.ConfigRepo.EventHandlerHistoryRepo.GetAsync(dto1.EventHandlerId, dto1.EventKey);

            Assert.Equal(dto1.EventHandlerId, fromRep.EventHandlerId);
            Assert.Equal(dto1.EventKey, fromRep.EventKey);
            Assert.Equal(dto1.EventSubscriptionId, fromRep.EventSubscriptionId);
            Assert.Equal(dto1.SubscriberId, fromRep.SubscriberId);

            var fromRepoForHandler = await Fixture.ConfigRepo.EventHandlerHistoryRepo.GetManyAsync(dto1.EventHandlerId);
            Assert.Equal(dto1.EventKey, fromRepoForHandler[0].EventKey);
            Assert.Equal(dto2.EventKey, fromRepoForHandler[1].EventKey);

        }

    }
}
