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
                    new Field{Name = "key", Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = true, IsSearchable = false, IsSortable = false },
                    new Field{Name = "removed", Type = typeof(bool).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = "type", Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = "log_index", Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = "transaction_hash", Type = typeof(string).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = false },
                    new Field{Name = "transaction_index", Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = true },
                    new Field{Name = "block_hash", Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = false },
                    new Field{Name = "block_number", Type = typeof(HexBigInteger).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = true },
                    new Field{Name = "address", Type = typeof(string).ToAzureDataType(), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = true },
                    new Field{Name = "data", Type = typeof(string).ToAzureDataType(), IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsKey = false, IsSearchable = false, IsSortable = false },
                    new Field{Name = "topics", Type = DataType.Collection(typeof(string).ToAzureDataType()), IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsKey = false, IsSearchable = true, IsSortable = false }
                }
            };

            return index;
        }

        public static Dictionary<string, object> Map(FilterLog filterLog)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("key", filterLog.Key().ToAzureFieldValue());
            dictionary.Add("removed", filterLog.Removed.ToAzureFieldValue());
            dictionary.Add("type", filterLog.Type.ToAzureFieldValue());
            dictionary.Add("log_index", filterLog.LogIndex.ToAzureFieldValue());
            dictionary.Add("transaction_hash", filterLog.TransactionHash.ToAzureFieldValue());
            dictionary.Add("transaction_index", filterLog.TransactionIndex.ToAzureFieldValue());
            dictionary.Add("block_hash", filterLog.BlockHash.ToAzureFieldValue());
            dictionary.Add("block_number", filterLog.BlockNumber.ToAzureFieldValue());
            dictionary.Add("address", filterLog.Address.ToAzureFieldValue());
            dictionary.Add("data", filterLog.Data.ToAzureFieldValue());
            dictionary.Add("topics", filterLog.Topics?.Where(t => t != null).Select(t => t.ToAzureFieldValue()).ToArray());

            return dictionary;
        }
    }
}
