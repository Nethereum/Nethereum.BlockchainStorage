using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class TransactionVmStack : TableEntity, ITransactionVmStackView
    {
        public TransactionVmStack()
        {
                
        }

        public TransactionVmStack(string address, string hash)
        {
            Address = address;
            TransactionHash = hash;
        }

        public string Address
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public string TransactionHash
        {
            get => RowKey;
            set => RowKey = value.ToRowKey();
        }

        public string StructLogs1 { get; set; } = string.Empty;
        public string StructLogs2 { get; set; } = string.Empty;
        public string StructLogs3 { get; set; } = string.Empty;
        public string StructLogs4 { get; set; } = string.Empty;
        public string StructLogs5 { get; set; } = string.Empty;
        public string StructLogs6 { get; set; } = string.Empty;
        public string StructLogs7 { get; set; } = string.Empty;
        public string StructLogs8 { get; set; } = string.Empty;
        public string StructLogs9 { get; set; } = string.Empty;
        public string StructLogs10 { get; set; } = string.Empty;
        public string StructLogs11 { get; set; } = string.Empty;
        public string StructLogs12 { get; set; } = string.Empty;
        public string StructLogs13 { get; set; } = string.Empty;
        public string StructLogs14 { get; set; } = string.Empty;
        public string StructLogs15 { get; set; } = string.Empty;
        public string StructLogs16 { get; set; } = string.Empty;
        public string StructLogs17 { get; set; } = string.Empty;
        public string StructLogs18 { get; set; } = string.Empty;
        public string StructLogs19 { get; set; } = string.Empty;
        public string StructLogs20 { get; set; } = string.Empty;
        public string StructLogs21 { get; set; } = string.Empty;
        public string StructLogs22 { get; set; } = string.Empty;
        public string StructLogs23 { get; set; } = string.Empty;
        public string StructLogs24 { get; set; } = string.Empty;
        public string StructLogs25 { get; set; } = string.Empty;

        public string StructLogs => ConcatStructLogs();

        private string ConcatStructLogs()
        {
            return GetStructLogs().ToString();
        }

        public static TransactionVmStack CreateTransactionVmStack(string transactionHash,
            string address,
            JObject stack)
        {
            var structsLogs = (JArray) stack["structLogs"];
            var transactionVmStack = new TransactionVmStack(address, transactionHash)
            {
                TransactionHash = transactionHash,
                Address = address
            };
            transactionVmStack.InitStruct(structsLogs);
            return transactionVmStack;
        }

        public JArray GetStructLogs()
        {
            var jarray = new JArray();

            for(int i = 1; i < 26; i++)
            {
                var property = GetType().GetProperty("StructLogs" + i);
                var val = property.GetValue(this) as string;
                if (!string.IsNullOrEmpty(val))
                {
                    foreach (var item in JArray.Parse(val))
                    {
                        jarray.Add(item);
                    }
                }
            }

            return jarray;
        }

        public void InitStruct(JArray structLogs)
        {
            var currentProperty = 1;
            var maxProperty = 15;
            var logProperty = new JArray();
            foreach (var structLog in structLogs)
            {
                logProperty.Add(structLog);
                //to make sure it fits in 64k just use 28k which * 2 is 56k (utf16)
                if (Encoding.Unicode.GetByteCount(logProperty.ToString()) > 56*1024)
                {
                    logProperty.Remove(structLog);
                    var property = GetType().GetProperty("StructLogs" + currentProperty);
                    property.SetValue(this, logProperty.ToString());

                    currentProperty = currentProperty + 1;
                    if (currentProperty <= maxProperty)
                    {
                        logProperty = new JArray();
                        logProperty.Add(structLog);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (currentProperty <= maxProperty)
            {
                var property = GetType().GetProperty("StructLogs" + currentProperty);
                property.SetValue(this, logProperty.ToString());
            }
        }
    }
}