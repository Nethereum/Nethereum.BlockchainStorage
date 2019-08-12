using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public class IndexDefinition
    {
        protected Dictionary<string, SearchField> FieldDictionary { get;}

        public IndexDefinition(string indexName = null, bool addPresetBlockchainFields = true)
        {
            IndexName = indexName;
            FieldDictionary = new Dictionary<string, SearchField>();

            if (addPresetBlockchainFields) LoadPresetBlockchainFields();

            LoadFieldDictionary();
            LoadFields();
        }

        protected virtual void LoadFieldDictionary() { }

        protected virtual void LoadFields()
        {
            Fields = FieldDictionary.Values.ToArray();
        }

        public SearchField[] Fields { get; set; } = Array.Empty<SearchField>();
        public string IndexName { get; protected set; }

        protected IndexDefinition(){}

        protected virtual void LoadPresetBlockchainFields() { }

        public IndexDefinition(string indexName){ IndexName = indexName; }

        public static void AddField(
            Dictionary<string, SearchField> fieldDictionary, 
            PresetSearchFieldName name, 
            Action<SearchField> fieldConfigurationAction)
        {
            var searchField = new SearchField(name);
            fieldConfigurationAction(searchField);
            fieldDictionary.Add(searchField.Name, searchField);
        }

        public static void AddField(
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
                    AddField(
                        fieldDictionary, 
                        structProperty.Item1, 
                        structProperty.Item2, 
                        parentProperties, 
                        searchFieldAttribute.Name, 
                        isCollection: true);
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


    }

}