namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class ContractQueryConfiguration
    {
        public ContractQueryParameter[] Parameters {get;set;} 
        public ContractAddressSource ContractAddressSource {get;set; }

        public string ContractABI {get;set; }
        public string FunctionSignature {get;set;}

        public string ContractAddress {get; set;}

        public int? ContractAddressParameterNumber {get;set;}

        public string ContractAddressStateVariableName {get;set;}

        public string EventStateOutputName {get;set;}
        public string SubscriptionStateOutputName {get;set;}
    }
}
