using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Storage.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class Contract : TableEntity, IContractView
    {

        public Contract()
        {
        }

        public Contract(string address)
        {
            RowKey = string.Empty;
            Address = address;
        }

        public string Address
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public string Name { get; set; } = string.Empty;

        public string ABI { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Code1 { get; set; } = string.Empty;

        public string Code2 { get; set; } = string.Empty;

        public string Code3 { get; set; } = string.Empty;

        public string Code5 { get; set; } = string.Empty;

        public string Code6 { get; set; } = string.Empty;

        public string Code7 { get; set; } = string.Empty;

        public string Code8 { get; set; } = string.Empty;

        public string Code9 { get; set; } = string.Empty;

        public string Code10 { get; set; } = string.Empty;

        public string Creator { get; set; } = string.Empty;

        public string TransactionHash { get; set; } = string.Empty;

        public static Contract CreateContract(string contractAddress, string code,
            RPC.Eth.DTOs.Transaction transactionSource)
        {
            var contract = new Contract(contractAddress)
            {
                Address = contractAddress,
                Creator = transactionSource.From,
                TransactionHash = transactionSource.TransactionHash
            };
            if(code != null) 
            { 
                contract.InitCode(code);
            }
            return contract;
        }

        public void InitCode(string code)
        {
            var codeArray = SplitByLength(code, 31000).ToArray();
            var max = codeArray.Length > 11 ? 11 : codeArray.Length;

            for (var i = 0; i < max; i++)
            {
                var property = i == 0 ? GetType().GetProperty("Code") : GetType().GetProperty("Code" + i);
                property.SetValue(this, codeArray[i]);
            }
        }

        public static IEnumerable<string> SplitByLength(string s, int length)
        {
            for (var i = 0; i < s.Length; i += length)
                if (i + length <= s.Length)
                    yield return s.Substring(i, length);
                else
                    yield return s.Substring(i);
        }
    }
}