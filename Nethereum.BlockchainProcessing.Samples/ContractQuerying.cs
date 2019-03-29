using Nethereum.BlockchainProcessing.BlockchainProxy;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class ContractQuerying
    {
        private static readonly string StandardContractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;

        [Fact]
        public async Task DynamicContractQuerying_NoParams_Returns_uint256()
        {
            var signature_totalSupply = "18160ddd";
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";
            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_totalSupply);
            Assert.IsType<BigInteger>(decoded);
        }

        [Fact]
        public async Task DynamicContractQuerying_NoParams_Returns_string()
        {
            var signature_name = "06fdde03";
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";

            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_name);
            Assert.Equal("JGX", decoded);  
        }

        [Fact]
        public async Task DynamicContractQuerying_AddressParam_Returns_uint256()
        {
            var signature_balanceOf = "70a08231"; 
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";
            var functionInput = new object[]{"0xa13210c21fbbed075ec210a71b477a81cb3da7d8"}; // _owner parameter - type: address

            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_balanceOf, functionInput);
            Assert.IsType<BigInteger>(decoded);
        }
    }
}
