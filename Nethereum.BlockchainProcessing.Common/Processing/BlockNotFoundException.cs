using System;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockNotFoundException: Exception
    {
        public BlockNotFoundException(BigInteger blockNumber):
            base($"Block is null - assumed to not yet exist.  BlockNumber: {blockNumber}"){}
    }
}
