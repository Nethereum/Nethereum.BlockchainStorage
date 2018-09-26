using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public static class Extensions
    {
        public static async Task<bool> IgnoreAsync<T>(this IEnumerable<IFilter<T>> filters, T item)
        {
            var match = await filters.IsMatchAsync(item);
            return !match;
        }

        public static async Task<bool> IsMatchAsync<T>(this IEnumerable<IFilter<T>> filters, T item)
        {
            if (!filters.Any()) return true;

            foreach (var filter in filters)
            {
                var match = await filter.IsMatchAsync(item);
                if (match) return true;
            }

            return false;
        }

        public static bool IsAnEmptyAddress(this string address)
        {
            if(string.IsNullOrEmpty(address))
                return true;

            return address == "0x0";
        }

        public static bool IsNotAnEmptyAddress(this string address)
        {
            return !address.IsAnEmptyAddress();
        }
    }
}
