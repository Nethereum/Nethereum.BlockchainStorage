using System;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockNotFoundException: Exception
    {
        public BlockNotFoundException(long blockNumber):
            base($"Block is null - assumed to not yet exist.  BlockNumber: {blockNumber}"){}
    }
}
