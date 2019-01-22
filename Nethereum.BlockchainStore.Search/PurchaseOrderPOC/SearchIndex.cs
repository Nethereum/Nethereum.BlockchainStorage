using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search.PurchaseOrderPOC
{
    public class SearchIndex
    {
        protected Type _eventType;
        protected EventAttribute _eventAttribute;

        public SearchField[] Fields { get; set; } = Array.Empty<SearchField>();
        public string IndexName { get; set; }
    }

    public class SearchIndex<TEvent> : SearchIndex where TEvent : IEventDTO, new()
    {
        public SearchIndex()
        {
            _eventType = typeof(TEvent);
            _eventAttribute = _eventType.GetCustomAttribute<EventAttribute>();
            if (_eventAttribute == null)
            {
                throw new ArgumentException("Event class does not have an EventAttribute");
            }

            //get SearchIndex attribute
            Searchable searchable = _eventType.GetCustomAttribute<Searchable>();
            IndexName = searchable == null ? _eventAttribute.Name : searchable.Name;

            var fieldDictionary = new Dictionary<string, SearchField>();

            LoadBlockchainLogFields(fieldDictionary);
            LoadIndexedFields(fieldDictionary);
            LoadNonIndexedFields(fieldDictionary);
            LoadCustomSearchFields(fieldDictionary);

            Fields = fieldDictionary.Values.ToArray();
        }

        private void LoadBlockchainLogFields(Dictionary<string, SearchField> fieldDictionary)
        {
            fieldDictionary.Add("log_key", new SearchField("log_key")
            {
                DataType = typeof(string),
                IsKey = true,
                IsSortable = true,
                LogValue = (filter) => 
                    $"{filter.BlockNumber.Value}_{filter.TransactionIndex.Value}_{filter.LogIndex.Value}"
            });

            fieldDictionary.Add("log_removed", new SearchField("log_removed")
            {
                DataType = typeof(bool),
                IsSortable = true,
                IsFilterable = true,
                LogValue = (filter) => filter.Removed
            });

            fieldDictionary.Add("log_type", new SearchField("log_type")
            {
                DataType = typeof(string),
                IsSortable = true,
                IsFilterable = true,
                LogValue = (filter) => filter.Type
            });

            fieldDictionary.Add("log_logIndex", new SearchField("log_logIndex")
            {
                DataType = typeof(HexBigInteger),
                IsFilterable = false,
                LogValue = (filter) => filter.LogIndex
            });
            
            fieldDictionary.Add("log_transactionHash", new SearchField("log_transactionHash")
            {
                DataType = typeof(string),
                IsSearchable = true,
                LogValue = (filter) => filter.TransactionHash
            });
            fieldDictionary.Add("log_transactionIndex", new SearchField("log_transactionIndex")
            {
                DataType = typeof(HexBigInteger),
                LogValue = (filter) => filter.TransactionIndex
            });
            fieldDictionary.Add("log_blockHash", new SearchField("log_blockHash")
            {
                DataType = typeof(string),
                LogValue = (filter) => filter.BlockHash
            });
            fieldDictionary.Add("log_blockNumber", new SearchField("log_blockNumber")
            {
                DataType = typeof(HexBigInteger),
                IsSearchable = true,
                IsSortable = true,
                IsFilterable = true,
                LogValue = (filter) => filter.BlockNumber
            });
            fieldDictionary.Add("log_address", new SearchField("log_address")
            {
                DataType = typeof(string),
                IsSearchable = true,
                IsSortable = true,
                IsFilterable = true,
                LogValue = (filter) => filter.Address
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
                    searchFieldAttribute.Name = nonIndexedProperty.parameter.Parameter.Name;
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
                IsFacetable = true
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

    public static class SearchIndexExtensions
    {
        public static SearchField Field(this SearchIndex searchIndex, string name)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.Name == name);
        }

        public static SearchField KeyField(this SearchIndex searchIndex)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.IsKey);
        }

    }

    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class Searchable : Attribute
    {
        public string Name { get; set; }
        public bool AddToIndex { get; set; } = true;
    }

    [AttributeUsage(validOn:AttributeTargets.Property)]
    public class SearchField: Attribute
    {
        public SearchField()
        {
            
        }

        public SearchField(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Type DataType { get; set; } = typeof(System.String);
        public bool IsKey { get; set; }
        public bool IsSearchable { get; set; } 
        public bool IsFilterable { get;set; }
        public bool IsSortable { get;set; }
        public bool IsFacetable { get; set; }
        public bool Ignore { get; set; }

        public PropertyInfo SourceProperty { get; set; }

        public Func<FilterLog, object> LogValue { get; set; }

        public object EventValue(IEventDTO eventDto)
        {
            return SourceProperty?.GetValue(eventDto);
        }
        
    }


}
