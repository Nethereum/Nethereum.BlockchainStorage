using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public class EventSearchIndexDefinition<TEvent> : SearchIndexDefinition where TEvent : class
    {
        private readonly Type _eventType;
        
        public EventSearchIndexDefinition(string indexName = null)
        {
            _eventType = typeof(TEvent);
            var eventAttribute = _eventType.GetCustomAttribute<EventAttribute>();
            if (eventAttribute == null)
            {
                throw new ArgumentException("Event class does not have an EventAttribute");
            }

            var searchable = _eventType.GetCustomAttribute<Searchable>();
            IndexName = indexName ?? (searchable == null ? eventAttribute.Name : searchable.Name);

            var fieldDictionary = new Dictionary<string, SearchField>();

            LoadBlockchainLogFields(fieldDictionary);
            LoadIndexedFields(fieldDictionary);
            LoadNonIndexedFields(fieldDictionary);
            LoadCustomSearchFields(fieldDictionary);

            Fields = fieldDictionary.Values.ToArray();
        }

        private void AddField(Dictionary<string, SearchField> fieldDictionary,
            PresetSearchFieldName name, Action<SearchField> fieldConfigurationAction)
        {
            var searchField = new SearchField(name);
            fieldConfigurationAction(searchField);
            fieldDictionary.Add(searchField.Name, searchField);
        }

        private void LoadBlockchainLogFields(Dictionary<string, SearchField> fields)
        {
            AddField(fields, PresetSearchFieldName.log_key, f =>
            {
                f.DataType = typeof(string);
                f.IsKey = true;
                f.IsSortable = true;
                f.LogValueCallback = (filter) =>
                    $"{filter.BlockNumber.Value}_{filter.TransactionIndex.Value}_{filter.LogIndex.Value}";
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

        private void LoadNonIndexedFields(Dictionary<string, SearchField> fieldDictionary)
        {
            var nonIndexedProperties = PropertiesExtractor
                .GetPropertiesWithParameterAttribute(_eventType)
                .Select(p => new { property = p, parameter = p.GetCustomAttribute<ParameterAttribute>() })
                .Where(p => p.parameter != null)
                .Where(p => p.parameter.Parameter.Indexed == false)
                .OrderBy(p => p.parameter.Order)
                .ToArray();

            foreach (var nonIndexedProperty in nonIndexedProperties)
            {
                var searchFieldAttribute =
                    nonIndexedProperty.property.GetCustomAttribute<SearchField>() ??
                    new SearchField();

                if (searchFieldAttribute.Ignore)
                {
                    continue;
                }
            
                searchFieldAttribute.SourceProperty = nonIndexedProperty.property;
                searchFieldAttribute.DataType = nonIndexedProperty.property.PropertyType;

                if (string.IsNullOrEmpty(searchFieldAttribute.Name))
                {
                    searchFieldAttribute.Name = nonIndexedProperty.property.Name;
                }

                fieldDictionary[searchFieldAttribute.Name] = searchFieldAttribute;
                
            }
        }

        private void LoadCustomSearchFields(Dictionary<string, SearchField> fieldDictionary)
        {
            var searchFields = _eventType
                .GetProperties()
                .Select(
                    p => new { property = p, searchField = p.GetCustomAttribute<SearchField>()})
                .Where(p => p.searchField != null);

            foreach (var propertySearchFieldPair in searchFields)
            {
                var searchFieldAttribute = propertySearchFieldPair.searchField;

                if (string.IsNullOrEmpty(searchFieldAttribute.Name))
                {
                    searchFieldAttribute.Name = propertySearchFieldPair.property.Name;
                }

                if (searchFieldAttribute.Ignore || 
                    fieldDictionary.ContainsKey(searchFieldAttribute.Name))
                {
                    continue;
                }

                searchFieldAttribute.SourceProperty = propertySearchFieldPair.property;
                searchFieldAttribute.DataType = propertySearchFieldPair.property.PropertyType;
           
                fieldDictionary[searchFieldAttribute.Name] = searchFieldAttribute;

            }
        }

        private void LoadIndexedFields(Dictionary<string, SearchField> fieldDictionary)
        {
            foreach (var topicField in MapTopicsToFields())
            {
                if (!topicField.Ignore)
                {
                    fieldDictionary.Add(topicField.Name, topicField);
                }
            }
        }

        private SearchField[] MapTopicsToFields()
        {
            var topics = new TopicFilterContainer<TEvent>();
            var fieldList = new List<SearchField>(3);

            if (topics.Topic1 != TopicFilter.Empty)
            {
                fieldList.Add(MapTopicToField(topics.Topic1));
            }
            if (topics.Topic2 != TopicFilter.Empty)
            {
                fieldList.Add(MapTopicToField(topics.Topic2));
            }
            if (topics.Topic3 != TopicFilter.Empty)
            {
                fieldList.Add(MapTopicToField(topics.Topic3));
            }

            return fieldList.ToArray();
        }

        private SearchField MapTopicToField(TopicFilter topic)
        {
           var field = topic.EventDtoProperty.GetCustomAttribute<SearchField>() ??
           new SearchField(topic.EventDtoProperty.Name)
            {
                IsSearchable = true,
                IsSortable = true,
                IsFilterable = true,
                IsFacetable = true,
                IsSuggester = true
            };

            field.SourceProperty = topic.EventDtoProperty;
            field.DataType = topic.EventDtoProperty.PropertyType;

            if(string.IsNullOrEmpty(field.Name))
            {
                field.Name = topic.EventDtoProperty.Name;
            }

            return field;
        }
    }
}
