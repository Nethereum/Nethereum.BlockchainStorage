namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class ContractQueryConfiguration
    {
        public ContractQueryParameter[] Parameters {get;set;} 
        public ContractAddressSource ContractAddressSource {get;set; }

        public string ContractABI {get;set; }
        public string FunctionSignature {get;set;}

        public string MetaDataOutputName {get;set;}


        public string ContractAddress {get; set;}

        public int ContractAddressParameterNumber {get;set;}

        public string ContractAddressMetadataVariableName {get;set;}

        public string EventSuscriptionStateVariableName {get;set;}
    }
}
