using Nethereum.ABI;
using Nethereum.ABI.Decoders;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IContractQuery
    {
        Task<object> Query(string contractAddress, string contractABI, string functionSignature, object[] functionInputs = null);
    }

    public class ContractQueryHelper : IContractQuery
    {
        private readonly Web3.Web3 web3;

        public ContractQueryHelper(Web3.Web3 web3)
        {
            this.web3 = web3;
        }
        public async Task<object> Query(string contractAddress, string contractABI, string functionSignature, object[] functionInputs = null)
        {
            var contract = web3.Eth.GetContract(contractABI, contractAddress);
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
