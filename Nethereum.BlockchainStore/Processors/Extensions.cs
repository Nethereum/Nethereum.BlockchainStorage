using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

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
            if (filters == null || !filters.Any()) return true;

            foreach (var filter in filters)
            {
                if(await filter.IsMatchAsync(item)) return true;
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

        public static bool IsForContractCreation(this Transaction transaction, TransactionReceipt transactionReceipt)
        {
            return transaction.To.IsAnEmptyAddress() && transactionReceipt.ContractAddress.IsNotAnEmptyAddress();
        }
    }
}
