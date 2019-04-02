using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureSubscriberSearchIndexFactory : ISubscriberSearchIndexFactory
    {
        EventToGenericSearchDocMapper eventToGenericSearchDocMapper = new EventToGenericSearchDocMapper();
        public AzureSubscriberSearchIndexFactory(
            IEventProcessingConfigurationDb eventProcessingConfigurationDb, 
            IAzureSearchService searchService)
        {
            EventProcessingConfigurationDb = eventProcessingConfigurationDb;
            SearchService = searchService;
        }

        public IEventProcessingConfigurationDb EventProcessingConfigurationDb { get; }
        public IAzureSearchService SearchService { get; }

        public async Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(long subscriberSearchIndexId)
        {
            var config = await EventProcessingConfigurationDb.GetSubscriberSearchIndexAsync(subscriberSearchIndexId);

            var index = eventToGenericSearchDocMapper.CreateAzureIndexDefinition(config.Name);
           
            var azureIndex = await SearchService.CreateEventIndexer(index, eventToGenericSearchDocMapper);
            return new AzureSubscriberSearchIndex(config.Name, azureIndex);
        }
    }

    public class EventToGenericSearchDocMapper : IEventToSearchDocumentMapper<List<ParameterOutput>, GenericEventLogSearchDocument>
    {
        public GenericEventLogSearchDocument Map(EventLog<List<ParameterOutput>> from)
        {
            var searchDoc = new GenericEventLogSearchDocument();

            searchDoc["DocumentKey"] = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}";
            searchDoc["BlockNumber"] = from.Log.BlockNumber?.Value.ToAzureFieldValue();
            searchDoc["TxHash"] = from.Log.TransactionHash;
            searchDoc["LogAddress"] = from.Log.Address;
            searchDoc["LogIndex"] = from.Log.LogIndex.Value.ToAzureFieldValue();

            if(from is DecodedEvent decodedEvent)
            {
                int count = 0;
                foreach(var key in decodedEvent.State.Keys)
                {
                    count ++;
                    searchDoc[$"stateVal{count}"] = $"{key}:{decodedEvent.State[key]?.ToAzureFieldValue()}";
                }   
            }

            foreach(var parameterOutput in from.Event.OrderBy(p => p.Parameter.Order))
            {
                var key = $"eventParam{parameterOutput.Parameter.Order}";
                var prefix = string.IsNullOrEmpty(parameterOutput.Parameter.Name) ? parameterOutput.Parameter.Order.ToString() : parameterOutput.Parameter.Name;
                searchDoc[key] = $"{prefix}:{parameterOutput.Result?.ToAzureFieldValue()}";
            }

            return searchDoc;
        }

        public Index CreateAzureIndexDefinition(string indexName)
        {
            var index = new Index 
            { 
                Name = indexName, 
                Fields = new List<Field>
                {
                    new Field("DocumentKey", DataType.String) { IsKey = true },

                    new Field("BlockNumber", DataType.String){IsSearchable = true },
                    new Field("TxHash", DataType.String){IsSearchable = true },
                    new Field("LogAddress", DataType.String){IsSearchable = true },
                    new Field("LogIndex", DataType.String),

                    new Field("eventParam1", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("eventParam2", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("eventParam3", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("eventParam4", DataType.String) {  },
                    new Field("eventParam5", DataType.String) { },
                    new Field("eventParam6", DataType.String) {  },
                    new Field("eventParam7", DataType.String) {  },
                    new Field("eventParam8", DataType.String) {  },
                    new Field("eventParam9", DataType.String) {  },
                    new Field("eventParam10", DataType.String) {  },

                    new Field("stateVal1", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("stateVal2", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("stateVal3", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("stateVal4", DataType.String) {  },
                    new Field("stateVal5", DataType.String) { },
                    new Field("stateVal6", DataType.String) {  },
                    new Field("stateVal7", DataType.String) {  },
                    new Field("stateVal8", DataType.String) {  },
                    new Field("stateVal9", DataType.String) {  },
                    new Field("stateVal10", DataType.String) {  }
                } 
            };
            return index;
        }
    }

    public class GenericEventLogSearchDocument : Dictionary<string, object>
    {
    }

    public class AzureSubscriberSearchIndex : ISubscriberSearchIndex
    {
        public AzureSubscriberSearchIndex(string name, IEventIndexer<List<ParameterOutput>> azureIndexer)
        {
            Name = name;
            AzureIndexer = azureIndexer;
        }
        public string Name {get; }

        public IEventIndexer<List<ParameterOutput>> AzureIndexer { get; }

        public async Task Index(DecodedEvent decodedEvent)
        {
            await AzureIndexer.IndexAsync(decodedEvent);
        }
    }
}
