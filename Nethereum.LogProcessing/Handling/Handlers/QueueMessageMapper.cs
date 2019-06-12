using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class QueueMessageMapper : IQueueMessageMapper
    {
        public static readonly IQueueMessageMapper Default = new QueueMessageMapper();

        public Func<DecodedEvent, object> MappingFunction { get; }
        public QueueMessageMapper(Func<DecodedEvent, object> mappingFunction = null)
        {
            MappingFunction = mappingFunction ?? new Func<DecodedEvent, object>((decodedEvent) => decodedEvent.ToQueueMessage()); 
        }

        public object Map(DecodedEvent decodedEvent) => MappingFunction(decodedEvent);
    }

}
