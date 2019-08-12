using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public class IndexDefinition<T> : IndexDefinition where T : class
    {
        public IndexDefinition(string indexName = null, bool addStandardBlockchainFields = true):
            base(indexName, addStandardBlockchainFields)
        {
            var eventType = typeof(T);
            var searchable = eventType.GetCustomAttribute<SearchIndex>();

            IndexName = indexName ?? searchable?.Name ?? eventType.Name;
        }

        protected override void LoadFieldDictionary()
        {
            LoadIndexedTopics(FieldDictionary);
            LoadFields(FieldDictionary, typeof(T));
        }

        protected virtual void LoadFields(
            Dictionary<string, SearchField> fieldDictionary, 
            Type type, 
            string prefix = null,
            List<PropertyInfo> parentProperties = null, 
            bool parentIsCollection = false)
        {
            var properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(
                    p => new
                    {
                        property = p, 
                        parameter = p.GetCustomAttribute<ParameterAttribute>(), 
                        searchField = p.GetCustomAttribute<SearchField>()
                    })
                .Where(p => p.parameter == null || p.parameter.Parameter.Indexed == false)
                .ToArray();

            foreach (var pair in properties)
            {
                var parents = parentProperties ?? new List<PropertyInfo>();

                var isCollection = parentIsCollection || pair.property.IsArrayOrListOfT();

                if (pair.searchField != null || pair.parameter != null)
                {
                    AddField(fieldDictionary, pair.property, pair.parameter, parents, prefix, isCollection);
                }
                else
                {
                    parents.Add(pair.property);

                    var childType = isCollection ? 
                        pair.property.GetItemTypeFromArrayOrListOfT() : 
                        pair.property.PropertyType;

                    var namePrefix = prefix == null ? pair.property.Name : $"{prefix}.{pair.property.Name}";

                    LoadFields(fieldDictionary, childType, namePrefix, parents, isCollection);
                }
            }

        }


        protected virtual void LoadIndexedTopics(Dictionary<string, SearchField> fieldDictionary)
        {
            foreach (var topicField in MapIndexedTopicsToFields())
            {
                if (!topicField.Ignore)
                {
                    fieldDictionary.Add(topicField.Name, topicField);
                }
            }
        }

        private static SearchField[] MapIndexedTopicsToFields()
        {
            var indexedTopics = PropertiesExtractor.GetIndexedTopics<T>();
            return indexedTopics.Select(t => MapTopicToField(t.PropertyInfo)).ToArray();
        }

        private static SearchField MapTopicToField(PropertyInfo property)
        {
            var field = property.GetCustomAttribute<SearchField>() ??
                        new SearchField(property.Name)
                        {
                            IsSearchable = true,
                            IsSortable = true,
                            IsFilterable = true,
                            IsFacetable = true,
                            IsSuggester = true
                        };

            field.SourceProperty = property;
            field.DataType = property.PropertyType;

            if(string.IsNullOrEmpty(field.Name))
            {
                field.Name = property.Name;
            }

            return field;
        }
    }

}