using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class FilterLogIndexUtil
    {
        public static Index Create(string name)
        {
            var index = new Index
            {
                Name = name.ToAzureIndexName(),
                Fields = new []{
                    new Field{Name = PresetSearchFieldName.log_key.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = true, IsSearchable = false, IsSortable = false },
                    new Field{Name = PresetSearchFieldName.log_removed.ToString(), Type = typeof(bool).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_type.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_log_index.ToString(), Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_transaction_hash.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = false },
                    new Field{Name = PresetSearchFieldName.log_transaction_index.ToString(), Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_block_hash.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = false },
                    new Field{Name = PresetSearchFieldName.log_block_number.ToString(), Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_address.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = true },
                    new Field{Name = PresetSearchFieldName.log_data.ToString(), Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = false },
                    new Field{Name = PresetSearchFieldName.log_topics.ToString(), Type = DataType.Collection(typeof(string).ToAzureDataType()), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = false }
                }
            };

            return index;
        }

        public static Dictionary<string, object> Map(FilterLog filterLog)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add(PresetSearchFieldName.log_key.ToString(), filterLog.Key().ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_removed.ToString(), filterLog.Removed.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_type.ToString(), filterLog.Type.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_log_index.ToString(), filterLog.LogIndex.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_transaction_hash.ToString(), filterLog.TransactionHash.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_transaction_index.ToString(), filterLog.TransactionIndex.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_block_hash.ToString(), filterLog.BlockHash.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_block_number.ToString(), filterLog.BlockNumber.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_address.ToString(), filterLog.Address.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_data.ToString(), filterLog.Data.ToAzureFieldValue());
            dictionary.Add(PresetSearchFieldName.log_topics.ToString(), filterLog.Topics?.Where(t => t != null).Select(t => t.ToAzureFieldValue()).ToArray());

            return dictionary;
        }
    }
}
