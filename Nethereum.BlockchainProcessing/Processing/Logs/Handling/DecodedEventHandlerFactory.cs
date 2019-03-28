using Nethereum.BlockchainProcessing.BlockchainProxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class DecodedEventHandlerFactory: IDecodedEventHandlerFactory
    {
        Dictionary<long, EventSubscriptionStateDto> _stateDictionary = new Dictionary<long, EventSubscriptionStateDto>();

        public DecodedEventHandlerFactory(IBlockchainProxyService blockchainProxy, IEventProcessingConfigurationDb configDb)
            :this(configDb, configDb, blockchainProxy,  configDb)
        {

        }

        public DecodedEventHandlerFactory(
            IEventSubscriptionStateFactory stateFactory, 
            IEventContractQueryConfigurationFactory contractQueryFactory,
            IContractQuery contractQueryHandler,
            IEventAggregatorConfigurationFactory eventAggregatorConfigurationFactory)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
            EventAggregatorConfigurationFactory = eventAggregatorConfigurationFactory;
        }

        public IEventSubscriptionStateFactory StateFactory { get; }
        public IEventContractQueryConfigurationFactory ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }
        public IEventAggregatorConfigurationFactory EventAggregatorConfigurationFactory { get; }

        public async Task<IDecodedEventHandler> CreateAsync(DecodedEventHandlerDto config)
        { 
            var state = await StateFactory.GetEventSubscriptionStateAsync(config.EventSubscriptionId);

            _stateDictionary[state.EventSubscriptionId] = state;

            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    return new EventRule(state);
                case EventHandlerType.Aggregate:
                    var aggregatorConfig = await EventAggregatorConfigurationFactory.GetEventAggregationConfiguration(config.Id);
                    return new EventAggregator(state, aggregatorConfig);
                case EventHandlerType.ContractQuery:
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(config.Id);
                    return new ContractQueryEventHandler(ContractQueryHandler, state, queryConfig);
                case EventHandlerType.Queue:
                    return new InMemoryEventQueue();
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
