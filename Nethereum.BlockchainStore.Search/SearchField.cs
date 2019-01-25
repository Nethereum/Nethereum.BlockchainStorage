using System;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
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

        public PropertyInfo SourceProperty { get; set; }

        public Func<FilterLog, object> LogValueCallback { get; set; }

        public object EventValue(IEventDTO eventDto)
        {
            return SourceProperty?.GetValue(eventDto);
        }

        public object GetValue<TEvent>(EventLog<TEvent> e)
        {
            return SourceProperty?.GetValue(e.Event) ??  LogValueCallback?.Invoke(e.Log);
        }
        
    }
}