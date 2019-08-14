using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class EventToGenericSearchDocMapper
    {
        public static class Fields
        {
            public const string DocumentKey = "document_key";
            public const string BlockNumber = "block_number";
            public const string TxHash = "transaction_hash";
            public const string LogAddress = "log_address";
            public const string LogIndex = "log_index";
            public const string StateValuePrefix = "state_value_";
            public const string EventParameterPrefix = "event_parameter_";

            public static string EventParameter(int order) => $"{EventParameterPrefix}{order}";
            public static string StateValue(int order) => $"{StateValuePrefix}{order}";

            public const int EventParameterCount = 10;
            public const int StateValueCount = 10;
        }

        public static GenericSearchDocument Map(EventLog<List<ParameterOutput>> from, Dictionary<string, object> stateValues)
        {
            var searchDoc = new GenericSearchDocument();

            searchDoc[Fields.DocumentKey] = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}";
            searchDoc[Fields.BlockNumber] = from.Log.BlockNumber?.Value.ToAzureFieldValue();
            searchDoc[Fields.TxHash] = from.Log.TransactionHash;
            searchDoc[Fields.LogAddress] = from.Log.Address;
            searchDoc[Fields.LogIndex] = from.Log.LogIndex.Value.ToAzureFieldValue();

            if (stateValues != null)
            {
                int count = 0;
                foreach (var key in stateValues.Keys)
                {
                    count++;
                    searchDoc[Fields.StateValue(count)] = $"{key}:{stateValues[key]?.ToAzureFieldValue()}";
                }
            }

            foreach (var parameterOutput in from.Event.OrderBy(p => p.Parameter.Order))
            {
                var key = Fields.EventParameter(parameterOutput.Parameter.Order);
                var prefix = string.IsNullOrEmpty(parameterOutput.Parameter.Name) ? parameterOutput.Parameter.Order.ToString() : parameterOutput.Parameter.Name;
                searchDoc[key] = $"{prefix}:{parameterOutput.Result?.ToAzureFieldValue()}";
            }

            return searchDoc;
        }

        public static Index CreateAzureIndexDefinition(string indexName)
        {
            var index = new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(Fields.DocumentKey, DataType.String) { IsKey = true },

                    new Field(Fields.BlockNumber, DataType.String){IsSearchable = true },
                    new Field(Fields.TxHash, DataType.String){IsSearchable = true },
                    new Field(Fields.LogAddress, DataType.String){IsSearchable = true },
                    new Field(Fields.LogIndex, DataType.String)
                }
            };

            for (var i = 1; i < (Fields.EventParameterCount + 1); i++)
            {
                bool searchAble = i < 4;
                index.Fields.Add(new Field(Fields.EventParameter(i), DataType.String) { IsSearchable = searchAble, IsFacetable = searchAble });
            }

            for (var i = 1; i < (Fields.StateValueCount + 1); i++)
            {
                bool searchAble = i < 4;
                index.Fields.Add(new Field(Fields.StateValue(i), DataType.String) { IsSearchable = searchAble, IsFacetable = searchAble });
            }

            return index;
        }
    }
}