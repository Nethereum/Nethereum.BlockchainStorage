using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Numerics;

namespace Nethereum.BlockchainStore.Search.Tests.TestData
{
    public static class Contracts
    {
        public static class StandardContract
        {

            [Function("transfer", "bool")]
            public class TransferFunction : FunctionMessage
            {
                [Parameter("address", "_to", 1)]
                public string To { get; set; }
                [Parameter("uint256", "_value", 2)]
                public BigInteger Value { get; set; }
            }


            [Event("Transfer")]
            public class TransferEvent : IEventDTO
            {
                [Parameter("address", "_from", 1, true)]
                public string From { get; set; }

                [Parameter("address", "_to", 2, true)]
                public string To { get; set; }

                [Parameter("uint256", "_value", 3, false)]
                public BigInteger Value { get; set; }
            }

            public static readonly string Abi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
            public static readonly string TransferEventSignature = "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
            public static readonly string ApprovalEventSignature = "8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925";

            public static readonly ContractABI ContractAbi = new ABIDeserialiser().DeserialiseContract(Abi);

            public static readonly EventABI ApprovalEventAbi = ContractAbi.Events.First(e => e.Sha3Signature == ApprovalEventSignature);

            public static readonly EventABI TransferEventAbi = ContractAbi.Events.First(e => e.Sha3Signature == TransferEventSignature);

            private static readonly JObject SampleTransferLogAsJObject = JObject.Parse(
                $@"{{
  'address': '0x243e72b69141f6af525a9a5fd939668ee9f2b354',
  'topics': [
    '0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef',
    '0x00000000000000000000000012890d2cce102216644c59dae5baed380d84830c',
    '0x00000000000000000000000013f022d72158410433cbd66f5dd8bf6d2d129924'
  ],
  'data': '0x00000000000000000000000000000000000000000000000000000000000003e8',
  'blockNumber': '0x36',
  'transactionHash': '0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd',
  'transactionIndex': '0x0',
  'blockHash': '0x58dab5a71037752b36e0a6af02f290fbc3dc5b2abf88d88f2c04defd9b8fb03b',
  'logIndex': '0x0',
  'removed': false
}}");

            public static FilterLog SampleTransferLog()
            {
                return SampleTransferLogAsJObject.ToObject<FilterLog>();
            }

            public static EventLog<TransferEvent> SampleTransferEventLog()
            {
                return SampleTransferLog().DecodeEvent<TransferEvent>();
            }
        }

    }
}
