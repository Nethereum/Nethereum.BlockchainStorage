using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

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

        public Func<TransactionWithReceipt, object> TxValueCallback { get; set; }

        public object GetValue(object dto)
        {
            if (dto == null) return null;

            if (ParentProperties.Count == 0)
            {
                var val = SourceProperty?.GetValue(dto);

                if (val == null) return null;

                if (val.IsArrayOrListOfT(out IEnumerable collection))
                {
                    return collection.GetItems();
                }

                return val;
            }

            object parentVal = dto;

            foreach (var parentProperty in ParentProperties)
            {
                parentVal = parentProperty.GetValue(parentVal);
                if (parentVal == null) break;
            }

            if (parentVal == null) return null;

            if (parentVal.IsArrayOrListOfT(out IEnumerable enumerable))
            {
                return enumerable.GetAllElementPropertyValues(SourceProperty);
            }

            var childVal = SourceProperty?.GetValue(parentVal);

            if (childVal == null) return null;

            if (childVal.IsArrayOrListOfT(out IEnumerable childEnumerable))
            {
                return childEnumerable.GetItems();
            }

            return childVal;
        }

        public object GetValue<TFunction>(FunctionCall<TFunction> functionCall) where TFunction : FunctionMessage, new()
        {
            if (TxValueCallback != null) return TxValueCallback.Invoke(functionCall.Tx);
            return GetValue(functionCall.Dto);
        }

        public object GetValue<TEvent>(EventLog<TEvent> e)
        {
            if (LogValueCallback != null) return LogValueCallback.Invoke(e.Log);

            return GetValue(e.Event);
        }
        
    }
}