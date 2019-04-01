namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventAggregatorConfiguration
    {
        public AggregatorSource Source {get;set;}
        public AggregatorOperation Operation {get;set;}
        public AggregatorDestination Destination {get;set;}
        public int EventParameterNumber {get;set;}
        public string InputName {get;set;}
        public string OutputName {get;set;}

    }
}
