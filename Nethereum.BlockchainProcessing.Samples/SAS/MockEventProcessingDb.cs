using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class MockEventProcessingDb
    {
        private static readonly string StandardContractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
        private static readonly string TransferEventSignature = "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

        public class MockDbDataRepository
        {
            public List<SubscriberDto> Subscribers = new List<SubscriberDto>();
            public List<ContractDto> Contracts = new List<ContractDto>();
            public List<EventSubscriptionDto> EventSubscriptions = new List<EventSubscriptionDto>();
            public List<EventSubscriptionAddressDto> EventAddresses = new List<EventSubscriptionAddressDto>();
            public List<DecodedEventHandlerDto> DecodedEventHandlers = new List<DecodedEventHandlerDto>();
            public List<ContractQueryDto> ContractQueries = new List<ContractQueryDto>();
            public List<ContractQueryParameterDto> ContractQueryParameters = new List<ContractQueryParameterDto>();
            public List<ParameterConditionDto> ParameterConditions = new List<ParameterConditionDto>();
            public List<EventAggregatorConfigurationDto> EventAggregators = new List<EventAggregatorConfigurationDto>();
            public Dictionary<long, EventSubscriptionStateDto> EventSubscriptionStates = new Dictionary<long, EventSubscriptionStateDto>();

            public EventAggregatorConfigurationDto Add(EventAggregatorConfigurationDto dto)
            {
                EventAggregators.Add(dto);
                return dto;
            }

            public DecodedEventHandlerDto Add(DecodedEventHandlerDto dto)
            {
                DecodedEventHandlers.Add(dto);
                return dto;
            }

            public SubscriberDto Add(SubscriberDto dto)
            {
                Subscribers.Add(dto);
                return dto;
            }

            public EventSubscriptionDto Add(EventSubscriptionDto dto)
            {
                EventSubscriptions.Add(dto);
                return dto;
            }

            public ContractDto Add(ContractDto dto)
            {
                Contracts.Add(dto);
                return dto;
            }

            public ContractQueryDto Add(ContractQueryDto dto)
            {
                ContractQueries.Add(dto);
                return dto;
            }

            public ContractQueryParameterDto Add(ContractQueryParameterDto dto)
            {
                ContractQueryParameters.Add(dto);
                return dto;
            }
        }

        public static IEventProcessingConfigurationDb CreateMockDb()
        {
            var repo = new MockDbDataRepository();

            const long Partition1 = 1;
            var idGenerator = new IdGenerator();

            AddHarry(Partition1, idGenerator, repo);
            AddGeorge(Partition1, idGenerator, repo);
            AddNosey(Partition1, idGenerator, repo);

            var db = MockAllQueries(repo);
      
            return db;
        }

        public static void AddNosey(long partitionId, IdGenerator id, MockDbDataRepository repo)
        { 
            var nosey = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                OrganisationName = "Nosey",
                PartitionId = partitionId
            });

            var catchAnyEventForAddressSubscription = repo.Add(
                nosey.CreateEventSubscription(id.Next<EventSubscriptionDto>()));

            repo.EventAddresses.Add(new EventSubscriptionAddressDto { 
                Id = id.Next<EventSubscriptionAddressDto>(), 
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id, 
                Address = "0x924442a66cfd812308791872c4b242440c108e19" });

            repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = catchAnyEventForAddressSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 1
            });
        }

        public static void AddGeorge(long partitionId, IdGenerator id, MockDbDataRepository repo)
        { 
            var george = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                OrganisationName = "George",
                PartitionId = partitionId
            });

            var standardContract = repo.Add(george.CreateContract(id.Next<ContractDto>(), "StandardContract", StandardContractAbi));
            var transferEventSubscription = repo.Add(george.CreateEventSubscription(id.Next<EventSubscriptionDto>(), standardContract.Id, TransferEventSignature));

            var transferCountHandler = repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferEventSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 1
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                Id = id.Next<EventAggregatorConfigurationDto>(),
                DecodedEventHandlerId = transferCountHandler.Id,
                Destination = AggregatorDestination.EventSubscriptionState,
                Operation = AggregatorOperation.Count,
                OutputName = "CurrentTransferCount"
            });

            repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferEventSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 2
            });
        }

        public static void AddHarry(long partitionId, IdGenerator id, MockDbDataRepository repo)
        { 
            var harry = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                OrganisationName = "Harry",
                PartitionId = partitionId
            });

            var standardContract = repo.Add(
                harry.CreateContract(id.Next<ContractDto>(), "StandardContract", StandardContractAbi));

            var transferSubscription = repo.Add(
                harry.CreateEventSubscription(id.Next<EventSubscriptionDto>(), standardContract.Id, TransferEventSignature));

            var transferValueRunningTotalHandler = repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.Aggregate,
                Order = 1
            });

            repo.Add(new EventAggregatorConfigurationDto
            {
                Id = id.Next<EventAggregatorConfigurationDto>(),
                DecodedEventHandlerId = transferValueRunningTotalHandler.Id,
                Destination = AggregatorDestination.EventSubscriptionState,
                Operation = AggregatorOperation.Sum,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = 3,
                OutputName = "RunningTotalForTransferValue"
            });

            var getTokenHandler = repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.ContractQuery,
                Order = 2
            });

            repo.Add(new ContractQueryDto
            {
                Id = id.Next<ContractQueryDto>(),
                DecodedEventHandlerId = getTokenHandler.Id,
                ContractId = standardContract.Id,
                ContractAddressSource = ContractAddressSource.EventAddress,
                EventStateOutputName = "TokenName",
                FunctionSignature = "06fdde03" // name
            });

            var getBalanceHandler = repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.ContractQuery,
                Order = 3
            });

            var getBalanceQuery = repo.Add(new ContractQueryDto
            {
                Id = id.Next<ContractQueryDto>(),
                DecodedEventHandlerId = getBalanceHandler.Id,
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

            repo.Add(new DecodedEventHandlerDto
            {
                Id = id.Next<DecodedEventHandlerDto>(),
                EventSubscriptionId = transferSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 4
            });
        }

        private static IEventProcessingConfigurationDb MockAllQueries(MockDbDataRepository repo)
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
                .Setup(d => d.GetEventAddressesAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(repo.EventAddresses.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetParameterConditionsAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(repo.ParameterConditions.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetDecodedEventHandlers(It.IsAny<long>()))
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
                    var contractQuery = repo.ContractQueries.FirstOrDefault(c => c.DecodedEventHandlerId == eventHandlerId);
                    if (contractQuery == null) throw new ArgumentException($"Could not find Contract Query Configuration for Event Handler Id: {eventHandlerId}");
                    var contract = repo.Contracts.FirstOrDefault(c => c.Id == contractQuery.ContractId);
                    if (contract == null) throw new ArgumentException($"Could not find Contract Query Id: {contractQuery.Id}, Contract Id: {contractQuery.ContractId}");
                    var parameters = repo.ContractQueryParameters.Where(p => p.ContractQueryId == contractQuery.Id);

                    ContractQueryConfiguration config = Map(contractQuery, contract, parameters);

                    return Task.FromResult(config);
                });

            configDb
                .Setup(d => d.GetEventAggregationConfiguration(It.IsAny<long>()))
                .Returns<long>((eventHandlerId) =>
                {
                    var dto = repo.EventAggregators.FirstOrDefault(c => c.DecodedEventHandlerId == eventHandlerId);
                    if (dto == null) throw new ArgumentException($"Could not find Event Aggregator Configuration for Event Handler Id: {eventHandlerId}");

                    EventAggregatorConfiguration config = Map(dto);

                    return Task.FromResult(config);
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

        private static ContractQueryConfiguration Map(ContractQueryDto contractQuery, ContractDto contract, IEnumerable<ContractQueryParameterDto> parameters)
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
