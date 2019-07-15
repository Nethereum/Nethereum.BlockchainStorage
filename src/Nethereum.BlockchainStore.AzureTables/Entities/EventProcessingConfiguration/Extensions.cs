using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public static class Extensions
    {
        public static long PartionKeyToLong(this TableEntity tableEntity)
        {
            if (string.IsNullOrWhiteSpace(tableEntity.PartitionKey)) return 0;
            return long.Parse(tableEntity.PartitionKey);
        }

        public static long RowKeyToLong(this TableEntity tableEntity)
        {
            if (string.IsNullOrWhiteSpace(tableEntity.RowKey)) return 0;
            return long.Parse(tableEntity.RowKey);
        }
    }
}
