using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public class Filter<T>: IFilter<T>
    {
        public Filter(Func<T, Task<bool>> matchFunc)
        {
            MatchFunc = matchFunc;
        }

        public Func<T, Task<bool>> MatchFunc { get; }

        public virtual async Task<bool> IsMatchAsync(T item)
        {
            return await MatchFunc(item);
        }
    }
}