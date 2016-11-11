using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class TransactionVmStack : TableEntityBase
    {
        public TransactionVmStack(AzureTable azureTable, DynamicTableEntity dynamicTableEntity = null)
            : base(azureTable, dynamicTableEntity)
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

        public string TransactionHash
        {
            get { return Get(string.Empty); }
            set
            {
                RowKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public string StructLogs1
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs2
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs3
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs4
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs5
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }


        public string StructLogs6
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs7
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs8
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs9
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs10
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs11
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs12
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs13
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs14
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs15
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs16
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs17
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs18
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs19
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs20
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs21
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs22
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs23
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs24
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string StructLogs25
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public static TransactionVmStack CreateTransactionVmStack(AzureTable transactionVmTable, string transactionHash,
            string address,
            JObject stack)
        {
            var structsLogs = (JArray) stack["structLogs"];
            var transactionVmStack = new TransactionVmStack(transactionVmTable)
            {
                TransactionHash = transactionHash,
                Address = address
            };
            transactionVmStack.InitStruct(structsLogs);
            return transactionVmStack;
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