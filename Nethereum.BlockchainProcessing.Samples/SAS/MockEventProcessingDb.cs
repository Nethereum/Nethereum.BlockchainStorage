using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public partial class MockEventProcessingDb
    {
        private static readonly string StandardContractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
        private static readonly string TransferEventSignature = "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

        public static IEventProcessingConfigurationDb Create(MockEventProcessingRepository repo)
        {
            const long PartitionId = 1;
            var idGenerator = new IdGenerator();

            AddHarry(PartitionId, idGenerator, repo);
            AddGeorge(PartitionId, idGenerator, repo);
            AddNosey(PartitionId, idGenerator, repo);
            AddTransferIndexer(PartitionId, idGenerator, repo);

            var db = MockAllQueries(repo);
      
            return db;
        }

        public static void AddTransferIndexer(long partitionId, IdGenerator id, MockEventProcessingRepository repo)
        { 
            var indexer = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Transfer Indexer",
                PartitionId = partitionId
            });

            var searchIndex = repo.Add(new SubscriberSearchIndexConfigurationDto
            {
                Id = id.Next<SubscriberSearchIndexConfigurationDto>(),
                SubscriberId = indexer.Id,
                Name = "subscriber-transfer-indexer"
            });

            var contract = repo.Add(new SubscriberContractDto
            {
                Id = id.Next<SubscriberContractDto>(),
                SubscriberId = indexer.Id,
                Abi = StandardContractAbi,
                Name = "StandardContract"
            });

            var catchAllSubscription = repo.Add( 
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = indexer.Id,
                    ContractId = contract.Id,
                    EventSignature = TransferEventSignature
                });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAllSubscription.Id,
                HandlerType = EventHandlerType.Index,
                Order = 2,
                SubscriberSearchIndexId = searchIndex.Id
            });
        }

        public static void AddNosey(long partitionId, IdGenerator id, MockEventProcessingRepository repo)
        { 
            var nosey = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Nosey",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = nosey.Id,
                Name = "subscriber-nosey"
            });

            var catchAnyEventForAddressSubscription = repo.Add( 
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = nosey.Id
                });

            repo.Add(new EventSubscriptionAddressDto { 
                Id = id.Next<EventSubscriptionAddressDto>(), 
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id, 
                Address = "0x924442a66cfd812308791872c4b242440c108e19" });

            var txHashTracker = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 1
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                EventHandlerId = txHashTracker.Id,
                Source = AggregatorSource.TransactionHash,
                Destination = AggregatorDestination.EventSubscriptionState,
                OutputName = "AllTransactionHashes",
                Operation = AggregatorOperation.AddToList
            });

            var blockTracker = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 2
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                EventHandlerId = blockTracker.Id,
                Source = AggregatorSource.BlockNumber,
                Destination = AggregatorDestination.EventSubscriptionState,
                OutputName = "AllBlockNumbers",
                Operation = AggregatorOperation.AddToList
            });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 2,
                SubscriberQueueId = queue.Id
            });
        }

        public static void AddGeorge(long partitionId, IdGenerator id, MockEventProcessingRepository repo)
        { 
            var george = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "George",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = george.Id,
                Name = "subscriber-george"
            });

            var standardContract = repo.Add(              
                new SubscriberContractDto
                {
                    Id = id.Next<SubscriberContractDto>(),
                    SubscriberId = george.Id,
                    Name = "StandardContract",
                    Abi = StandardContractAbi
                });

            var transferEventSubscription = repo.Add(
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = george.Id,
                    ContractId = standardContract.Id,
                    EventSignature = TransferEventSignature
                });

            var transferCountHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferEventSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 1
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                Id = id.Next<EventAggregatorConfigurationDto>(),
                EventHandlerId = transferCountHandler.Id,
                Destination = AggregatorDestination.EventSubscriptionState,
                Operation = AggregatorOperation.Count,
                OutputName = "CurrentTransferCount"
            });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferEventSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 2,
                SubscriberQueueId = queue.Id
            });
        }

        public static void AddHarry(long partitionId, IdGenerator id, MockEventProcessingRepository repo)
        { 
            var harry = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Harry",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = harry.Id,
                Name = "subscriber-harry"
            });

            var standardContract = repo.Add(
                      new SubscriberContractDto
                      {
                          Id = id.Next<SubscriberContractDto>(),
                          SubscriberId = harry.Id,
                          Name = "StandardContract",
                          Abi = StandardContractAbi
                      });

            var transferSubscription = repo.Add(
                    new EventSubscriptionDto
                    {
                        Id = id.Next<EventSubscriptionDto>(),
                        SubscriberId = harry.Id,
                        ContractId = standardContract.Id,
                        EventSignature = TransferEventSignature
                    });
                    
            var getTransactionHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.GetTransaction,
                Order = 0
            });

            var transferValueRunningTotalHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 1
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                Id = id.Next<EventAggregatorConfigurationDto>(),
                EventHandlerId = transferValueRunningTotalHandler.Id,
                Destination = AggregatorDestination.EventSubscriptionState,
                Operation = AggregatorOperation.Sum,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = 3,
                OutputName = "RunningTotalForTransferValue"
            });

            var getTokenHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.ContractQuery,
                Order = 2
            });

            repo.Add(new ContractQueryDto
            {
                Id = id.Next<ContractQueryDto>(),
                EventHandlerId = getTokenHandler.Id,
                ContractId = standardContract.Id,
                ContractAddressSource = ContractAddressSource.EventAddress,
                EventStateOutputName = "TokenName",
                FunctionSignature = "06fdde03" // name
            });

            var getBalanceHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.ContractQuery,
                Order = 3
            });

            var getBalanceQuery = repo.Add(new ContractQueryDto
            {
                Id = id.Next<ContractQueryDto>(),
                EventHandlerId = getBalanceHandler.Id,
                ContractId = 1,
                ContractAddressSource = ContractAddressSource.EventAddress,
                EventStateOutputName = "FromAddressCurrentBalance",
                FunctionSignature = "70a08231" // balanceOf
            });

            repo.Add(new ContractQueryParameterDto
            {
                Id = id.Next<ContractQueryParameterDto>(),
                ContractQueryId = getBalanceQuery.Id,
                Order = 1,
                Source = EventValueSource.EventParameters,
                EventParameterNumber = 1
            });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 4,
                SubscriberQueueId = queue.Id
            });
        }

        private static IEventProcessingConfigurationDb MockAllQueries(MockEventProcessingRepository repo)
        {
            Mock<IEventProcessingConfigurationDb> configDb = new Mock<IEventProcessingConfigurationDb>();

            configDb
                .Setup(d => d.GetSubscribersAsync(It.IsAny<long>()))
                .Returns<long>((partitionId) => Task.FromResult(repo.Subscribers.Where(s => s.PartitionId == partitionId).ToArray()));

            configDb
                .Setup(d => d.GetEventSubscriptionsAsync(It.IsAny<long>()))
                .Returns<long>((subscriberId) => Task.FromResult(repo.EventSubscriptions.Where(s => s.SubscriberId == subscriberId).ToArray()));

            configDb
                .Setup(d => d.GetContractAsync(It.IsAny<long>()))
                .Returns<long>((contractId) => Task.FromResult(repo.Contracts.Where(s => s.Id == contractId).FirstOrDefault()));

            configDb
                .Setup(d => d.GetEventSubscriptionAddressesAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(repo.EventSubscriptionAddresses.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetParameterConditionsAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(repo.ParameterConditions.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetEventHandlers(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(repo.DecodedEventHandlers.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetEventSubscriptionStateAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) =>
                {
                    if (!repo.EventSubscriptionStates.ContainsKey(eventSubscriptionId))
                    {
                        repo.EventSubscriptionStates.Add(eventSubscriptionId, new EventSubscriptionStateDto(eventSubscriptionId));
                    }
                    return Task.FromResult(repo.EventSubscriptionStates[eventSubscriptionId]);
                });

            configDb
                .Setup(d => d.SaveAsync(It.IsAny<EventSubscriptionStateDto>()))
                .Callback<EventSubscriptionStateDto>((state) => repo.EventSubscriptionStates[state.EventSubscriptionId] = state)
                .Returns(Task.CompletedTask);

            configDb
                .Setup(d => d.GetContractQueryConfigurationAsync(It.IsAny<long>()))
                .Returns<long>((eventHandlerId) =>
                {
                    var contractQuery = repo.ContractQueries.FirstOrDefault(c => c.EventHandlerId == eventHandlerId);
                    if (contractQuery == null) throw new ArgumentException($"Could not find Contract Query Configuration for Event Handler Id: {eventHandlerId}");
                    var contract = repo.Contracts.FirstOrDefault(c => c.Id == contractQuery.ContractId);
                    if (contract == null) throw new ArgumentException($"Could not find Contract Query Id: {contractQuery.Id}, Contract Id: {contractQuery.ContractId}");
                    var parameters = repo.ContractQueryParameters.Where(p => p.ContractQueryId == contractQuery.Id);

                    ContractQueryConfiguration config = Map(contractQuery, contract, parameters);

                    return Task.FromResult(config);
                });

            configDb
                .Setup(d => d.GetEventAggregationConfigurationAsync(It.IsAny<long>()))
                .Returns<long>((eventHandlerId) =>
                {
                    var dto = repo.EventAggregators.FirstOrDefault(c => c.EventHandlerId == eventHandlerId);
                    if (dto == null) throw new ArgumentException($"Could not find Event Aggregator Configuration for Event Handler Id: {eventHandlerId}");

                    EventAggregatorConfiguration config = Map(dto);

                    return Task.FromResult(config);
                });

            configDb
                .Setup(d => d.GetSubscriberQueueAsync(It.IsAny<long>()))
                .Returns<long>((subscriberQueueId) =>
                {
                    var dto = repo.SubscriberQueues.FirstOrDefault(q => q.Id == subscriberQueueId);
                    if (dto == null) throw new ArgumentException($"Could not find Subscriber Queue Id: {subscriberQueueId}");
                    return Task.FromResult(dto);
                });

            configDb
                .Setup(d => d.GetSubscriberSearchIndexAsync(It.IsAny<long>()))
                .Returns<long>((subscriberSearchIndexId) => {
                    var dto = repo.SubscriberSearchIndexes.FirstOrDefault(q => q.Id == subscriberSearchIndexId);
                    if (dto == null) throw new ArgumentException($"Could not find Subscriber Search Index Id: {subscriberSearchIndexId}");
                    return Task.FromResult(dto);
                });
                

            return configDb.Object;
        }

        private static EventAggregatorConfiguration Map(EventAggregatorConfigurationDto dto)
        {
            return new EventAggregatorConfiguration
            {
                Destination = dto.Destination,
                EventParameterNumber = dto.EventParameterNumber,
                InputName = dto.InputName,
                Operation = dto.Operation,
                OutputName = dto.OutputName,
                Source = dto.Source
            };
        }

        private static ContractQueryConfiguration Map(ContractQueryDto contractQuery, SubscriberContractDto contract, IEnumerable<ContractQueryParameterDto> parameters)
        {
            return new ContractQueryConfiguration
            {
                ContractABI = contract.Abi,
                ContractAddress = contractQuery.ContractAddress,
                ContractAddressParameterNumber = contractQuery.ContractAddressParameterNumber,
                ContractAddressSource = contractQuery.ContractAddressSource,
                ContractAddressStateVariableName = contractQuery.ContractAddressStateVariableName,
                EventStateOutputName = contractQuery.EventStateOutputName,
                FunctionSignature = contractQuery.FunctionSignature,
                SubscriptionStateOutputName = contractQuery.SubscriptionStateOutputName,
                Parameters = parameters.Select(p => new ContractQueryParameter
                {
                    Order = p.Order,
                    EventParameterNumber = p.EventParameterNumber,
                    EventStateName = p.EventStateName,
                    Source = p.Source,
                    Value = p.Value
                }).ToArray()
            };
        }

        public class IdGenerator
        {
            Dictionary<Type, long> _counters = new Dictionary<Type, long>();
            public long Next<T>()
            {
                long id = 1;
                if(_counters.TryGetValue(typeof(T), out long val))
                {
                    id = val + 1;
                }
                _counters[typeof(T)] =  id;
                return id;
            }
        }
    }
}
