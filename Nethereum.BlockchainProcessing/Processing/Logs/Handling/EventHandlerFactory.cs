using Nethereum.BlockchainProcessing.BlockchainProxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class EventHandlerFactory: IEventHandlerFactory
    {
        Dictionary<long, EventSubscriptionStateDto> _stateDictionary = new Dictionary<long, EventSubscriptionStateDto>();

        public EventHandlerFactory(
            IBlockchainProxyService blockchainProxy, 
            IEventProcessingConfigurationDb configDb, 
            ISubscriberQueueFactory subscriberQueueFactory,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory)
            :this(
                 configDb, 
                 configDb, 
                 blockchainProxy,  
                 configDb, 
                 blockchainProxy, 
                 subscriberQueueFactory,
                 subscriberSearchIndexFactory)
        {
        }

        public EventHandlerFactory(
            IEventSubscriptionStateFactory stateFactory, 
            IEventContractQueryConfigurationFactory contractQueryFactory,
            IContractQuery contractQueryHandler,
            IEventAggregatorConfigurationFactory eventAggregatorConfigurationFactory,
            IGetTransactionByHash getTransactionProxy,
            ISubscriberQueueFactory subscriberQueueFactory,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
            EventAggregatorConfigurationFactory = eventAggregatorConfigurationFactory;
            GetTransactionProxy = getTransactionProxy;
            SubscriberQueueFactory = subscriberQueueFactory;
            SubscriberSearchIndexFactory = subscriberSearchIndexFactory;
        }

        public IEventSubscriptionStateFactory StateFactory { get; }
        public IEventContractQueryConfigurationFactory ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }
        public IEventAggregatorConfigurationFactory EventAggregatorConfigurationFactory { get; }
        public IGetTransactionByHash GetTransactionProxy { get; }
        public ISubscriberQueueFactory SubscriberQueueFactory { get; }
        public ISubscriberSearchIndexFactory SubscriberSearchIndexFactory { get; }

        public async Task<IEventHandler> CreateAsync(EventHandlerDto config)
        { 
            var state = await StateFactory.GetEventSubscriptionStateAsync(config.EventSubscriptionId);

            _stateDictionary[state.EventSubscriptionId] = state;

            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    return new EventRule(state);
                case EventHandlerType.Aggregate:
                    var aggregatorConfig = await EventAggregatorConfigurationFactory.GetEventAggregationConfigurationAsync(config.Id);
                    return new EventAggregator(state, aggregatorConfig);
                case EventHandlerType.ContractQuery:
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(config.Id);
                    return new ContractQueryEventHandler(ContractQueryHandler, state, queryConfig);
                case EventHandlerType.Queue:
                    var queue = await SubscriberQueueFactory.GetSubscriberQueueAsync(config.SubscriberQueueId);
                    return new QueueHandler(queue);
                case EventHandlerType.GetTransaction:
                    return new GetTransactionEventHandler(GetTransactionProxy);
                case EventHandlerType.Index:
                    var searchIndex = await SubscriberSearchIndexFactory.GetSubscriberSearchIndexAsync(config.SubscriberSearchIndexId);
                    return new SearchIndexHandler(searchIndex);
                default:
                    throw new ArgumentException("unsupported handler type");
            }
        }

        public async Task SaveStateAsync()
        {
            foreach(var state in _stateDictionary.Values)
            {
                await StateFactory.SaveAsync(state);
            }
        }
    }
}
