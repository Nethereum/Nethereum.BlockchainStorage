using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class Contract : TableEntityBase
    {
        private static List<Contract> cachedContracts;

        public Contract(AzureTable at, DynamicTableEntity dte = null) : base(at, dte)
        {
            RowKey = string.Empty;
        }

        public string Address
        {
            get { return Get(string.Empty); }
            set
            {
                PartitionKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public string Name
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string ABI
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code1
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code2
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code3
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code5
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code6
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code7
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code8
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code9
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Code10
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Creator
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string TransactionHash
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public static Contract CreateContract(AzureTable contractTable, string contractAddress, string code,
            RPC.Eth.DTOs.Transaction transactionSource)
        {
            var contract = new Contract(contractTable)
            {
                Address = contractAddress,
                Creator = transactionSource.From,
                TransactionHash = transactionSource.TransactionHash
            };
            contract.InitCode(code);

            cachedContracts?.Add(contract);

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

        public static async Task<Contract> FindAsync(AzureTable table, string contractAddress)
        {
            if (cachedContracts != null) return cachedContracts.FirstOrDefault(x => x.Address == contractAddress);

            var tr =
                await
                    table.ExecuteAsync(TableOperation.Retrieve(contractAddress.ToLowerInvariant().HtmlEncode(),
                        string.Empty)).ConfigureAwait(false);
            if ((HttpStatusCode) tr.HttpStatusCode != HttpStatusCode.NotFound)
                return new Contract(table, (DynamicTableEntity) tr.Result);

            return null;
        }

        public static async Task InitContractsCacheAsync(CloudTable table)
        {
            if (cachedContracts != null)
                cachedContracts = await FindAllAsync(table).ConfigureAwait(false);
        }

        public static async Task<List<Contract>> FindAllAsync(AzureTable table)
        {
            var tableQuery = new TableQuery();
            var contracts = new List<Contract>();
            for (var chunker = table.CreateQueryChunker(tableQuery); chunker.HasMore;)
                foreach (var dte in await chunker.TakeAsync().ConfigureAwait(false))
                    contracts.Add(new Contract(table, dte));
            return contracts;
        }

        public static async Task<bool> ExistsAsync(AzureTable table, string contractAddress)
        {
            var contract = await FindAsync(table, contractAddress).ConfigureAwait(false);
            if (contract != null) return true;
            return false;
        }
    }
}