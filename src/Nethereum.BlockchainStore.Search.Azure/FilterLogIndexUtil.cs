using Microsoft.Azure.Search.Models;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class FilterLogIndexUtil
    {
        private static Field[] GetPresetAzureFields() => PresetSearchFields
            .LogFields
            .Select(f => f.ToAzureField())
            .ToArray();

        public static Index Create(string name)
        {
            var index = new Index
            {
                Name = name.ToAzureIndexName(),
                Fields = GetPresetAzureFields()
            };

            return index;
        }

        public static GenericSearchDocument Map(FilterLog filterLog)
        {
            return filterLog.ToAzureDocument(PresetSearchFields.LogFields); 
        }
    }
}
