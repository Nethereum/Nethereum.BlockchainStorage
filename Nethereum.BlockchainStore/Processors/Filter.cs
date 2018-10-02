using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public class Filter<T>: IFilter<T>
    {
        public Filter(Func<T, Task<bool>> condition)
        {
            Condition = condition;
        }

        public Func<T, Task<bool>> Condition { get; }

        public virtual async Task<bool> IsMatchAsync(T item)
        {
            return await Condition(item);
        }
    }
}