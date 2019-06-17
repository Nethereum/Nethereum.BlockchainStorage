using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs
{
    /// <summary>
    /// A case insensitive list of unique addresses
    /// </summary>
    public class UniqueAddressList: HashSet<string>
    {
        public UniqueAddressList():base(StringComparer.OrdinalIgnoreCase){ }
    }
}
