using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.EFCore
{
    public interface IBlockchainDbContextFactory
    {
        BlockchainDbContextBase CreateContext();
    }
}