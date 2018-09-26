using System;

namespace Nethereum.BlockchainStore.Processors
{
    public class Filter<T>
    {
        public Filter(Func<T, bool> matchFunc)
        {
            MatchFunc = matchFunc;
        }

        public Func<T, bool> MatchFunc { get; }

        public virtual bool IsMatch(T item)
        {
            return MatchFunc(item);
        }
    }
}