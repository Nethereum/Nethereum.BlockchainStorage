using Nethereum.BlockchainProcessing.BlockchainProxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public interface IDecodedEventHandlerConfigurationFactory
    {
        Task<EventSubscriptionStateDto> GetEventSubscriptionStateAsync(long eventSubscriptionId);
        Task SaveAsync(EventSubscriptionStateDto state);
    }

    public class DecodedEventHandlerFactory: IDecodedEventHandlerFactory
    {
        Dictionary<long, EventSubscriptionStateDto> _stateDictionary = new Dictionary<long, EventSubscriptionStateDto>();

        public DecodedEventHandlerFactory(
            IEventSubscriptionStateFactory stateFactory, 
            IEventContractQueryFactory contractQueryFactory,
            IContractQuery contractQueryHandler)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
        }

        public IEventSubscriptionStateFactory StateFactory { get; }
        public IEventContractQueryFactory ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }

        public async Task<IDecodedEventHandler> CreateAsync(DecodedEventHandlerDto config)
        { 
            var state = await StateFactory.GetEventSubscriptionStateAsync(config.EventSubscriptionId);

            _stateDictionary[state.EventSubscriptionId] = state;

            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    return new EventRule(state);
                case EventHandlerType.Aggregate:
                    return new EventAggregator(state);
                case EventHandlerType.ContractQuery:
                    var queryConfig = await ContractQueryFactory.GetContractQueryAsync(config.Id);
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
