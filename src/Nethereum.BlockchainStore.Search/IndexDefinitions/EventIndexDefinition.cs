using System;
using System.Collections.Generic;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Reflection;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;

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

        protected override void LoadGenericBlockchainFields()
        {
            var fields = FieldDictionary;

            AddField(fields, PresetSearchFieldName.log_key, f =>
            {
                f.DataType = typeof(string);
                f.IsKey = true;
                f.IsSortable = true;
                f.LogValueCallback = (filter) =>
                    $"{filter.Key()}";
            });

            AddField(fields, PresetSearchFieldName.log_removed, f =>
            {
                f.DataType = typeof(bool);
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Removed;
            });

            AddField(fields, PresetSearchFieldName.log_type, f =>
            {
                f.DataType = typeof(string);
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Type;
            });

            AddField(fields, PresetSearchFieldName.log_log_index, f => 
            {
                f.DataType = typeof(HexBigInteger);
                f.IsFilterable = false;
                f.LogValueCallback = (filter) => filter.LogIndex;
            });
            
            AddField(fields, PresetSearchFieldName.log_transaction_hash, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.LogValueCallback = (filter) => filter.TransactionHash;
                f.IsSuggester = true;
            });

            AddField(fields, PresetSearchFieldName.log_transaction_index, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.LogValueCallback = (filter) => filter.TransactionIndex;
            });

            AddField(fields, PresetSearchFieldName.log_block_hash, f =>
            {
                f.DataType = typeof(string);
                f.LogValueCallback = (filter) => filter.BlockHash;
            });

            AddField(fields, PresetSearchFieldName.log_block_number, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.BlockNumber;
                f.IsSuggester = true;
            });

            AddField(fields, PresetSearchFieldName.log_address, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Address;
                f.IsSuggester = true;
            });

        }
    }
}
