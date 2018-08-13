using Nethereum.Blockchain.EFCore;

namespace Nethereum.BlockchainStore.EFCore
{
    public interface IBlockchainDbContextFactory
    {
        BlockchainDbContextBase CreateContext();
    }
}