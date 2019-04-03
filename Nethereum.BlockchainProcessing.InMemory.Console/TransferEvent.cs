using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    partial class Program
    {
        /*
         * Solidity Excerpt
         * event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
        */
        [Event("Transfer")]
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_tokenId", 3, true)]
            public BigInteger Value {get; set;}
        }

    }
}
