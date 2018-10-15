using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    [Function("buyApprenticeChest")]
    public class BuyApprenticeFunction: FunctionMessage
    {
        [Parameter("uint256", "_region", 1)]
        public BigInteger Region { get; set; }
    }

    [Function("openChest")]
    public class OpenChestFunction: FunctionMessage
    {
        [Parameter("uint256", "_identifier", 1)]
        public BigInteger Identifier { get; set; }
    }
}
