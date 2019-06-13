using Nethereum.ABI;
using Nethereum.ABI.Decoders;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing
{
    public class ContractQueryHelper : IContractQuery
    {
        private readonly IEthApiContractService eth;

        public ContractQueryHelper(Web3.IWeb3 web3):this(web3.Eth)
        {
        }

        public ContractQueryHelper(IEthApiContractService ethApiContractService)
        {
            this.eth = ethApiContractService;
        }

        public async Task<object> Query(string contractAddress, string contractABI, string functionSignature, object[] functionInputs = null)
        {
            var contract = eth.GetContract(contractABI, contractAddress);
            var functionAbi =  contract.ContractBuilder.ContractABI.Functions.FirstOrDefault(f => f.Sha3Signature == functionSignature);
            var function = new Function(contract, new FunctionBuilder(contractAddress, functionAbi));

            var callInput = functionInputs != null && functionAbi.InputParameters.Any() ? 
                function.CreateCallInput(functionInputs) : 
                function.CreateCallInput();

            //var defaultResult = await function.CallDecodingToDefaultAsync(callInput); //returns null
            var bytesResult = await function.CallRawAsync(callInput);

            var returnParameter = functionAbi.OutputParameters.FirstOrDefault();

            if(returnParameter == null) return null;

            var abiType = returnParameter.ABIType;

            if(abiType is StringType)
            {
                return new StringBytes32Decoder().Decode(bytesResult);
            }

            var decoded = abiType.Decode(bytesResult, abiType.GetDefaultDecodingType());
            return decoded;
        }

    }
}
