//using Microsoft.Azure.Search.Models;
//using Nethereum.ABI.FunctionEncoding;
//using Nethereum.BlockchainProcessing.Processing.Logs;
//using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
//using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
//using Nethereum.Contracts;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Nethereum.BlockchainStore.Search.Azure
//{
//    public class AzureSubscriberSearchIndexFactory : ISubscriberSearchIndexFactory
//    {
//        EventToGenericSearchDocMapper eventToGenericSearchDocMapper = new EventToGenericSearchDocMapper();
//        public AzureSubscriberSearchIndexFactory(
//            IAzureSearchService searchService)
//        {
//            SearchService = searchService;
//        }

//        public IEventProcessingConfigurationRepository EventProcessingConfigurationDb { get; }
//        public IAzureSearchService SearchService { get; }

//        public async Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(ISubscriberSearchIndexDto config)
//        {
//            var index = eventToGenericSearchDocMapper.CreateAzureIndexDefinition(config.Name);
//            var azureIndex = await SearchService.CreateEventIndexer(index, eventToGenericSearchDocMapper).ConfigureAwait(false);
//            return new AzureSubscriberSearchIndex(config.Name, azureIndex);
//        }
//    }

//    public class EventToGenericSearchDocMapper : IEventToSearchDocumentMapper<List<ParameterOutput>, GenericEventLogSearchDocument>
//    {
//        public static class Fields
//        {
//            public const string DocumentKey = "document_key";
//            public const string BlockNumber = "block_number";
//            public const string TxHash = "transaction_hash";
//            public const string LogAddress = "log_address";
//            public const string LogIndex = "log_index";
//            public const string StateValuePrefix = "state_value_";
//            public const string EventParameterPrefix = "event_parameter_";

//            public static string EventParameter(int order) => $"{EventParameterPrefix}{order}";
//            public static string StateValue(int order) => $"{StateValuePrefix}{order}";

//            public const int EventParameterCount = 10;
//            public const int StateValueCount = 10;
//        }

//        public GenericEventLogSearchDocument Map(EventLog<List<ParameterOutput>> from)
//        {
//            var searchDoc = new GenericEventLogSearchDocument();

//            searchDoc[Fields.DocumentKey] = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}";
//            searchDoc[Fields.BlockNumber] = from.Log.BlockNumber?.Value.ToAzureFieldValue();
//            searchDoc[Fields.TxHash] = from.Log.TransactionHash;
//            searchDoc[Fields.LogAddress] = from.Log.Address;
//            searchDoc[Fields.LogIndex] = from.Log.LogIndex.Value.ToAzureFieldValue();

//            if (from is DecodedEvent decodedEvent)
//            {
//                int count = 0;
//                foreach (var key in decodedEvent.State.Keys)
//                {
//                    count++;
//                    searchDoc[Fields.StateValue(count)] = $"{key}:{decodedEvent.State[key]?.ToAzureFieldValue()}";
//                }
//            }

//            foreach (var parameterOutput in from.Event.OrderBy(p => p.Parameter.Order))
//            {
//                var key = Fields.EventParameter(parameterOutput.Parameter.Order);
//                var prefix = string.IsNullOrEmpty(parameterOutput.Parameter.Name) ? parameterOutput.Parameter.Order.ToString() : parameterOutput.Parameter.Name;
//                searchDoc[key] = $"{prefix}:{parameterOutput.Result?.ToAzureFieldValue()}";
//            }

//            return searchDoc;
//        }

//        public Index CreateAzureIndexDefinition(string indexName)
//        {
//            var index = new Index
//            {
//                Name = indexName,
//                Fields = new List<Field>
//                {
//                    new Field(Fields.DocumentKey, DataType.String) { IsKey = true },

//                    new Field(Fields.BlockNumber, DataType.String){IsSearchable = true },
//                    new Field(Fields.TxHash, DataType.String){IsSearchable = true },
//                    new Field(Fields.LogAddress, DataType.String){IsSearchable = true },
//                    new Field(Fields.LogIndex, DataType.String)
//                }
//            };

//            for(var i = 1; i< (Fields.EventParameterCount + 1); i++) 
//            {
//                bool searchAble = i < 4;
//                index.Fields.Add(new Field(Fields.EventParameter(i), DataType.String) { IsSearchable = searchAble, IsFacetable = searchAble });
//            }

//            for (var i = 1; i < (Fields.StateValueCount + 1); i++)
//            {
//                bool searchAble = i < 4;
//                index.Fields.Add(new Field(Fields.StateValue(i), DataType.String) { IsSearchable = searchAble, IsFacetable = searchAble });
//            }

//            return index;
//        }
//    }

//    public class GenericEventLogSearchDocument : Dictionary<string, object>
//    {
//    }

//    public class AzureSubscriberSearchIndex : ISubscriberSearchIndex
//    {
//        public AzureSubscriberSearchIndex(string name, IEventIndexer<List<ParameterOutput>> azureIndexer)
//        {
//            Name = name;
//            AzureIndexer = azureIndexer;
//        }
//        public string Name { get; }

//        public IEventIndexer<List<ParameterOutput>> AzureIndexer { get; }

//        public async Task Index(DecodedEvent decodedEvent)
//        {
//            await AzureIndexer.IndexAsync(decodedEvent);
//        }
//    }
//}