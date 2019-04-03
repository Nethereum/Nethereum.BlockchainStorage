using System;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockNotFoundException: Exception
    {
        public BlockNotFoundException(ulong blockNumber):
            base($"Block is null - assumed to not yet exist.  BlockNumber: {blockNumber}"){}
    }
}
