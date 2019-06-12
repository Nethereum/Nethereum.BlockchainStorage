using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class ContractQueryConfiguration
    {
        public IContractQueryParameterDto[] Parameters {get;set;} 

        public ISubscriberContractDto Contract { get;set;}

        public IContractQueryDto Query { get;set;}
    }
}
