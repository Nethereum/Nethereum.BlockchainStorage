using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public class EventIndexDefinition<TEvent> : IndexDefinition<TEvent> where TEvent : class
    {
        public EventIndexDefinition(string indexName = null, bool addPresetEventLogFields = true):
            base(indexName, addPresetEventLogFields)
        {
            var eventType = typeof(TEvent);
            var eventAttribute = eventType.GetCustomAttribute<EventAttribute>();
            var searchable = eventType.GetCustomAttribute<SearchIndex>();

            IndexName = indexName ?? searchable?.Name ?? eventAttribute?.Name ?? eventType.Name;
        }

        protected override void LoadPresetBlockchainFields()
        {
            FieldDictionary.AddPresetFilterLogFields();
        }
    }
}
