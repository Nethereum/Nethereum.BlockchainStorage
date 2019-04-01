namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class ContractQueryParameter
    {
        public int Order {get;set;}
        public EventValueSource Source {get;set;}
        public object Value {get;set;}
        public int EventParameterNumber {get;set;}
        public string EventStateName {get;set;}
    }
}
