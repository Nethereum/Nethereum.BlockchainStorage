using System;
using System.Net;
using System.Text;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public static class Extensions
    {
        public static int Limit = 64000;

        public static bool IsWithinAzureTableStorageLimit(this string val)
        {
            return Encoding.Unicode.GetByteCount(val ?? string.Empty) < Limit;
        }

        public static string RestrictToAzureTableStorageLimit(this string val, Func<string, string> callbackIfTooLong)
        {
            if (val == null) return string.Empty;

            return val.IsWithinAzureTableStorageLimit() ? val : callbackIfTooLong(val);
        }

        public static string RestrictToAzureTableStorageLimit(this string val, string valIfTooLong = "")
        {
            return val.RestrictToAzureTableStorageLimit((v) => valIfTooLong);
        }

        public static string ToPartitionKey(this string val)
        {
            return val == null ? string.Empty : WebUtility.HtmlEncode(val.ToLowerInvariant());
        }

        public static string ToRowKey(this string val)
        {
            return val.ToPartitionKey();
        }
    }
}
