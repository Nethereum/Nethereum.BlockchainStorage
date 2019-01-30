using System;

namespace Nethereum.BlockchainStore.Search
{
    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class SearchIndex : Attribute
    {
        public SearchIndex()
        {
                
        }

        public SearchIndex(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}