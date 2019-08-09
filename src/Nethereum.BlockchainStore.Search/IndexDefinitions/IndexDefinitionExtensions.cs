using System.Collections.Generic;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.Search
{
    public static class IndexDefinitionExtensions
    {
        public static void AddGenericBlockchainFields(this IndexDefinition definition, Dictionary<string, SearchField> fields)
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