using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs
{
    /// <summary>
    /// A case insensitive list of unique transaction hashes
    /// </summary>
    public class UniqueTransactionHashList : HashSet<string>
    {
        public UniqueTransactionHashList():base(StringComparer.OrdinalIgnoreCase){}
    }
}
