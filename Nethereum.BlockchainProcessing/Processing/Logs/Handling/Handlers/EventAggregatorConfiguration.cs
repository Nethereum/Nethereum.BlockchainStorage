using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class EventAggregatorConfiguration
    {
        public AggregatorSource Source {get;set;}
        public AggregatorOperation Operation {get;set;}
        public AggregatorDestination Destination {get;set;}
        public int EventParameterNumber {get;set;}
        public string SourceKey {get;set;}
        public string OutputKey {get;set;}

    }
}
