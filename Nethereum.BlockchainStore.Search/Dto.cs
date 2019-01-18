using System;
using System.Collections.Generic;
using System.Text;

namespace Nethereum.BlockchainStore.Search
{
    public class Block
    {
        public string Number { get; set; }
    }

    public class Transaction
    {
        public string Hash { get; set; }
    }

    public class Address: AddressBase
    {
        
    }

    public class Contract: AddressBase
    {
        
    }

    public abstract class AddressBase
    {
        public string Address { get; set; }
    }
}
