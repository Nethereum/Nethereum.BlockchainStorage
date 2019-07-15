using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public class IndexDefinition<T> : IndexDefinition where T : class
    {
        public IndexDefinition(string indexName = null, bool addStandardBlockchainFields = true)
        {
            var eventType = typeof(T);
            var searchable = eventType.GetCustomAttribute<SearchIndex>();

            IndexName = indexName ?? searchable?.Name ?? eventType.Name;

            LoadFields(addStandardBlockchainFields);
        }

        protected static void AddField(Dictionary<string, SearchField> fieldDictionary,
            PresetSearchFieldName name, Action<SearchField> fieldConfigurationAction)
        {
            var searchField = new SearchField(name);
            fieldConfigurationAction(searchField);
            fieldDictionary.Add(searchField.Name, searchField);
        }

        protected void LoadFields(bool addStandardBlockchainFields)
        {
            var fieldDictionary = new Dictionary<string, SearchField>();

            if (addStandardBlockchainFields)
            {
                LoadGenericBlockchainFields(fieldDictionary);
            }

            LoadIndexedTopics(fieldDictionary);
            LoadFields(fieldDictionary, typeof(T));

            Fields = fieldDictionary.Values.ToArray();
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

        protected virtual void LoadGenericBlockchainFields(Dictionary<string, SearchField> fields){}

        private void AddField(
            Dictionary<string, SearchField> fieldDictionary, 
            PropertyInfo property, 
            ParameterAttribute parameter = null, 
            List<PropertyInfo> parentProperties = null,  
            string prefix = null, 
            bool isCollection = false)
        {
            var searchFieldAttribute =
                property.GetCustomAttribute<SearchField>() ??
                new SearchField();

            if (searchFieldAttribute.Ignore)
            {
                return;
            }

            if (searchFieldAttribute.IsKey)
            {
                foreach (var field in fieldDictionary.Values)
                {
                    field.IsKey = false;
                }
            }

            if (parentProperties == null)
            {
                parentProperties = new List<PropertyInfo>();
            }

            searchFieldAttribute.ParentProperties.AddRange(parentProperties);

            searchFieldAttribute.SourceProperty = property;
            searchFieldAttribute.DataType = property.PropertyType;
            searchFieldAttribute.IsCollection = isCollection || property.PropertyType.IsArrayOrListOfT();
            
            if (string.IsNullOrEmpty(searchFieldAttribute.Name))
            {
                searchFieldAttribute.Name = prefix == null ? property.Name : $"{prefix}.{property.Name}";
            }

            if (parameter?.IsTuple() ?? false)
            {
                var structProperties = GetChildProperties(property.PropertyType);

                parentProperties.Add(property);

                foreach (var structProperty in structProperties)
                {
                    AddField(fieldDictionary, structProperty.Item1, structProperty.Item2, parentProperties, searchFieldAttribute.Name);
                }
            }
            else if (parameter?.IsTupleArray() ?? false)
            {
                Type structType = property.GetItemTypeFromArrayOrListOfT();

                if (structType == null) return;

                var structProperties = GetChildProperties(structType);

                parentProperties.Add(property);

                foreach (var structProperty in structProperties)
                {
                    AddField(fieldDictionary, structProperty.Item1, structProperty.Item2, parentProperties, searchFieldAttribute.Name, isCollection: true);
                }
            }
            else
            {
                fieldDictionary[searchFieldAttribute.Name] = searchFieldAttribute;
            }
        }

        private static Tuple<PropertyInfo, ParameterAttribute>[] GetChildProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new Tuple<PropertyInfo, ParameterAttribute>(p, p.GetCustomAttribute<ParameterAttribute>()))
                .OrderBy(p => p.Item2?.Order)
                .ToArray();
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

        private SearchField[] MapIndexedTopicsToFields()
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

    public class IndexDefinition
    {
        public SearchField[] Fields { get; set; } = Array.Empty<SearchField>();
        public string IndexName { get; protected set; }

        protected IndexDefinition()
        {
        }

        public IndexDefinition(string indexName)
        {
            IndexName = indexName;
        }
    }
}