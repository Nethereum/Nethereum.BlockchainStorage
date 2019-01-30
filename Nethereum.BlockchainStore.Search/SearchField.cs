using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    /// <summary>
    /// Indicates the value of the property should be added to a search index
    /// Allows configuration of the field within the search index
    /// Do not use on complex objects - instead place the attribute on the properties within the complex objects
    /// </summary>
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

        public SearchField(PresetSearchFieldName fieldName)
        {
            Name = fieldName.ToString();
        }

        public string Name { get; set; }
        public Type DataType { get; set; } = typeof(System.String);
        public bool IsKey { get; set; }
        public bool IsSearchable { get; set; } 
        public bool IsFilterable { get;set; }
        public bool IsSortable { get;set; }
        public bool IsFacetable { get; set; }
        public bool Ignore { get; set; }
        public bool IsSuggester { get;set; }
        public int SuggesterOrder { get; set; }
        public bool IsCollection { get; set; }

        public List<PropertyInfo> ParentProperties { get;set; } = new List<PropertyInfo>();

        public PropertyInfo SourceProperty { get; set; }

        public Func<FilterLog, object> LogValueCallback { get; set; }

        public object EventValue(object eventDto)
        {
            if (eventDto == null) return null;

            if (ParentProperties.Count == 0)
            {
                var val = SourceProperty?.GetValue(eventDto);

                if (val == null) return null;

                if(val.IsArrayOrList(out IEnumerable collection))
                {
                    return collection.GetElementsAsArray();
                }

                return val;
            }

            object parentVal = eventDto;

            foreach (var parentProperty in ParentProperties)
            {
                parentVal = parentProperty.GetValue(parentVal);
                if (parentVal == null) break;
            }

            if (parentVal == null) return null;

            if(parentVal.IsArrayOrList(out IEnumerable enumerable))
            {
                return enumerable.GetPropertyValues(SourceProperty);
            }

            return SourceProperty?.GetValue(parentVal);
        }

        public object GetValue<TEvent>(EventLog<TEvent> e)
        {
            if (LogValueCallback != null) return LogValueCallback.Invoke(e.Log);

            return EventValue(e.Event);
        }
        
    }
}