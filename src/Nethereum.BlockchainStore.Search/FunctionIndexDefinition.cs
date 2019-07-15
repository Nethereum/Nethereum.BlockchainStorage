using System;
using System.Collections.Generic;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;

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


        protected override void LoadGenericBlockchainFields(Dictionary<string, SearchField> fields)
        {
            AddField(fields, PresetSearchFieldName.tx_uid, f =>
            {
                f.DataType = typeof(string);
                f.IsKey = true;
                f.IsSortable = true;
                f.IsSearchable = true;
                f.TxValueCallback = (tx) =>
                    $"{tx.BlockNumber.Value}_{tx.Transaction.TransactionIndex.Value}";
            });

            AddField(fields, PresetSearchFieldName.tx_hash, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsFilterable = true;
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.TransactionHash;
            });

            AddField(fields, PresetSearchFieldName.tx_index, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.IsFilterable = false;
                f.TxValueCallback = (tx) => tx.Transaction.TransactionIndex;
            });

            AddField(fields, PresetSearchFieldName.tx_block_hash, f => 
            {
                f.DataType = typeof(string);
                f.TxValueCallback = (tx) => tx.Transaction.BlockHash;
            });
            
            AddField(fields, PresetSearchFieldName.tx_block_number, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.BlockNumber;
                f.IsSuggester = true;
            });

            AddField(fields, PresetSearchFieldName.tx_block_timestamp, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.BlockTimestamp;
            });

            AddField(fields, PresetSearchFieldName.tx_value, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.Value;
            });

            AddField(fields, PresetSearchFieldName.tx_from, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.From?.ToLower();
            });

            AddField(fields, PresetSearchFieldName.tx_to, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.To?.ToLower();
            });

            AddField(fields, PresetSearchFieldName.tx_gas, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.Gas;
            });

            AddField(fields, PresetSearchFieldName.tx_gas_price, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.GasPrice;
            });

            AddField(fields, PresetSearchFieldName.tx_input, f =>
            {
                f.DataType = typeof(string);
                f.TxValueCallback = (tx) => tx.Transaction.Input;
            });

            AddField(fields, PresetSearchFieldName.tx_nonce, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.TxValueCallback = (tx) => tx.Transaction.Nonce;
            });



        }
    }
}