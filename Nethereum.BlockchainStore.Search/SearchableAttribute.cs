using System;

namespace Nethereum.BlockchainStore.Search
{
    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class Searchable : Attribute
    {
        public string Name { get; set; }
    }
}