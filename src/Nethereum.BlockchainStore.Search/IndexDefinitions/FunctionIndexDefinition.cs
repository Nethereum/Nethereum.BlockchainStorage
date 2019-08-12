using System;
using System.Collections.Generic;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.BlockchainStore.Search
{
    public class FunctionIndexDefinition<TFunction> : 
        IndexDefinition<TFunction> where TFunction : class
    {
        public FunctionIndexDefinition(string indexName = null, bool addPresetTransactionFields = true):
            base(indexName, addPresetTransactionFields)
        {
            var eventType = typeof(TFunction);
            var functionAttribute = eventType.GetCustomAttribute<FunctionAttribute>();
            var searchable = eventType.GetCustomAttribute<SearchIndex>();

            IndexName = indexName ?? searchable?.Name ?? functionAttribute?.Name ?? eventType.Name;
        }

        protected override void LoadPresetBlockchainFields()
        {
            FieldDictionary.AddPresetTransactionFields();
        }

    }
}