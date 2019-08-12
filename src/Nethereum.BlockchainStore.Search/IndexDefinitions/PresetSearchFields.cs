using System.Collections.Generic;
using System.Linq;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.Search
{
    public static class PresetSearchFields
    {
        public static SearchField[] LogFields =
            CreatePresetFilterLogFields()
            .Values
            .ToArray();

        public static Dictionary<string, SearchField> CreatePresetFilterLogFields()
        {
            var fields = new Dictionary<string, SearchField>();
            fields.AddPresetFilterLogFields();
            return fields;
        }

        public static void AddPresetFilterLogFields(this Dictionary<string, SearchField> fields)
        {
            IndexDefinition.AddField(fields, PresetSearchFieldName.log_key, f =>
            {
                f.DataType = typeof(string);
                f.IsKey = true;
                f.IsSortable = true;
                f.LogValueCallback = (filter) =>
                    $"{filter.Key()}";
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_removed, f =>
            {
                f.DataType = typeof(bool);
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Removed;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_type, f =>
            {
                f.DataType = typeof(string);
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Type;
                f.IsFilterable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_log_index, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.LogIndex;
                f.IsFilterable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_transaction_hash, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.LogValueCallback = (filter) => filter.TransactionHash;
                f.IsSuggester = true;
                f.IsFacetable = true;
                f.IsFilterable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_transaction_index, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.LogValueCallback = (filter) => filter.TransactionIndex;
                f.IsFilterable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_block_hash, f =>
            {
                f.DataType = typeof(string);
                f.LogValueCallback = (filter) => filter.BlockHash;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_block_number, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.BlockNumber;
                f.IsSuggester = true;
                f.IsFacetable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_address, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Address;
                f.IsSuggester = true;
                f.IsFacetable = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_data, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = false;
                f.IsSortable = false;
                f.IsFilterable = false;
                f.LogValueCallback = (filter) => filter.Data;
                f.IsSuggester = false;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.log_topics, f =>
            {
                f.DataType = typeof(string);
                f.IsCollection = true;
                f.IsSearchable = true;
                f.IsSortable = false;
                f.IsFilterable = true;
                f.LogValueCallback = (filter) => filter.Topics;
                f.IsSuggester = false;
                f.IsFacetable = true;
            });

        }

        public static void AddPresetTransactionFields(this Dictionary<string, SearchField> fields)
        {
            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_uid, f =>
            {
                f.DataType = typeof(string);
                f.IsKey = true;
                f.IsSortable = true;
                f.IsSearchable = true;
                f.TxValueCallback = (tx) =>
                    $"{tx.BlockNumber.Value}_{tx.Transaction.TransactionIndex.Value}";
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_hash, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsFilterable = true;
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.TransactionHash;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_index, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.IsFilterable = false;
                f.TxValueCallback = (tx) => tx.Transaction.TransactionIndex;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_block_hash, f =>
            {
                f.DataType = typeof(string);
                f.TxValueCallback = (tx) => tx.Transaction.BlockHash;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_block_number, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.BlockNumber;
                f.IsSuggester = true;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_block_timestamp, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.BlockTimestamp;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_value, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.Value;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_from, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.From?.ToLower();
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_to, f =>
            {
                f.DataType = typeof(string);
                f.IsSearchable = true;
                f.IsSortable = true;
                f.IsFilterable = true;
                f.IsFacetable = true;
                f.TxValueCallback = (tx) => tx.Transaction.To?.ToLower();
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_gas, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.Gas;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_gas_price, f =>
            {
                f.DataType = typeof(HexBigInteger);
                f.IsSortable = true;
                f.TxValueCallback = (tx) => tx.Transaction.GasPrice;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_input, f =>
            {
                f.DataType = typeof(string);
                f.TxValueCallback = (tx) => tx.Transaction.Input;
            });

            IndexDefinition.AddField(fields, PresetSearchFieldName.tx_nonce, f =>
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