using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public static class EventProcessingConfigMock
    {
        private static readonly string StandardContractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
        private static readonly string TransferEventSignature = "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
        public static readonly string ApprovalEventSignature = "8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925";


        public static EventProcessingConfigContext Create(long partitionId, out IdGenerator idGenerator)
        {
            EventProcessingConfigContext repo = new EventProcessingConfigContext();

            idGenerator = new IdGenerator();

            AddHarry(partitionId, idGenerator, repo);
            AddGeorge(partitionId, idGenerator, repo);
            AddNosey(partitionId, idGenerator, repo);
            AddIan(partitionId, idGenerator, repo);
            AddEric(partitionId, idGenerator, repo);
            AddCharlie(partitionId, idGenerator, repo);
            AddMary(partitionId, idGenerator, repo);

            return repo;
        }

        public static void AddIan(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        { 
            var indexer = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Ian",
                PartitionId = partitionId
            });

            var searchIndex = repo.Add(new SubscriberSearchIndexConfigurationDto
            {
                Id = id.Next<SubscriberSearchIndexConfigurationDto>(),
                SubscriberId = indexer.Id,
                Name = "subscriber-transfer-ian"
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
                    EventSignatures = new List<string>(new []{TransferEventSignature })
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

        public static void AddCharlie(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        {
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Charlie",
                PartitionId = partitionId
            });

            var repository = repo.Add(new SubscriberRepositoryConfigurationDto
            {
                Id = id.Next<SubscriberRepositoryConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "charlielogs"
            });

            var contract = repo.Add(new SubscriberContractDto
            {
                Id = id.Next<SubscriberContractDto>(),
                SubscriberId = subscriber.Id,
                Abi = StandardContractAbi,
                Name = "StandardContract"
            });

            var catchAllSubscription = repo.Add(
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = subscriber.Id,
                    ContractId = contract.Id,
                    EventSignatures = new List<string>(new []{TransferEventSignature })
                });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAllSubscription.Id,
                HandlerType = EventHandlerType.Store,
                Order = 1,
                SubscriberRepositoryId = repository.Id
            });
        }

        public static void AddEric(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        {
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Eric",
                PartitionId = partitionId
            });

            var contract = repo.Add(new SubscriberContractDto
            {
                Id = id.Next<SubscriberContractDto>(),
                SubscriberId = subscriber.Id,
                Abi = StandardContractAbi,
                Name = "StandardContract"
            });

            var catchAllTransfersSubscription = repo.Add(
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = subscriber.Id,
                    ContractId = contract.Id,
                    EventSignatures = new List<string>(new []{TransferEventSignature })
                });

            var ruleHandler = repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAllTransfersSubscription.Id,
                HandlerType = EventHandlerType.Rule,
                Order = 1
            });

            repo.Add(new EventRuleConfigurationDto
            {
                Id = id.Next<EventRuleConfigurationDto>(),
                EventHandlerId = ruleHandler.Id,
                Type = EventRuleType.Modulus,
                Source = EventRuleSource.EventSubscriptionState,
                SourceKey = "EventsHandled",
                Value = "10"
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "subscriber-transfer-eric"
            });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = catchAllTransfersSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 2,
                SubscriberQueueId = queue.Id
            });
        }

        public static void AddNosey(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        { 
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Nosey",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "subscriber-nosey"
            });

            var catchAnyEventForAddressSubscription = repo.Add( 
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = subscriber.Id
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
                OutputKey = "AllTransactionHashes",
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
                OutputKey = "AllBlockNumbers",
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

        public static void AddGeorge(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        { 
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "George",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "subscriber-george"
            });

            var standardContract = repo.Add(              
                new SubscriberContractDto
                {
                    Id = id.Next<SubscriberContractDto>(),
                    SubscriberId = subscriber.Id,
                    Name = "StandardContract",
                    Abi = StandardContractAbi
                });

            var transferEventSubscription = repo.Add(
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = subscriber.Id,
                    ContractId = standardContract.Id,
                    EventSignatures = new List<string>(new[] { TransferEventSignature })
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
                OutputKey = "CurrentTransferCount"
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

        public static void AddMary(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        {
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Mary",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "subscriber-mary"
            });

            var standardContract = repo.Add(
                new SubscriberContractDto
                {
                    Id = id.Next<SubscriberContractDto>(),
                    SubscriberId = subscriber.Id,
                    Name = "StandardContract",
                    Abi = StandardContractAbi
                });

            var anyEventSubscription = repo.Add(
                new EventSubscriptionDto
                {
                    Id = id.Next<EventSubscriptionDto>(),
                    SubscriberId = subscriber.Id,
                    ContractId = standardContract.Id,
                    CatchAllContractEvents = true
                });

            repo.Add(new EventHandlerDto
            {
                Id = id.Next<EventHandlerDto>(),
                EventSubscriptionId = anyEventSubscription.Id,
                HandlerType = EventHandlerType.Queue,
                Order = 1,
                SubscriberQueueId = queue.Id
            });
        }

        public static void AddHarry(long partitionId, IdGenerator id, EventProcessingConfigContext repo)
        { 
            var subscriber = repo.Add(new SubscriberDto
            {
                Id = id.Next<SubscriberDto>(),
                Disabled = false,
                Name = "Harry",
                PartitionId = partitionId
            });

            var queue = repo.Add(new SubscriberQueueConfigurationDto
            {
                Id = id.Next<SubscriberQueueConfigurationDto>(),
                SubscriberId = subscriber.Id,
                Name = "subscriber-harry"
            });

            var standardContract = repo.Add(
                      new SubscriberContractDto
                      {
                          Id = id.Next<SubscriberContractDto>(),
                          SubscriberId = subscriber.Id,
                          Name = "StandardContract",
                          Abi = StandardContractAbi
                      });

            var transferSubscription = repo.Add(
                    new EventSubscriptionDto
                    {
                        Id = id.Next<EventSubscriptionDto>(),
                        SubscriberId = subscriber.Id,
                        ContractId = standardContract.Id,
                        EventSignatures = new List<string>(new[] { TransferEventSignature })
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
                OutputKey = "RunningTotalForTransferValue"
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
    }
}
