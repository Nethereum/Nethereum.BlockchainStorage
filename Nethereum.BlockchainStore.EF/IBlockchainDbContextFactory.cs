using Nethereum.BlockchainStore.EF;

namespace Nethereum.BlockchainStore.EF
{
    public interface IBlockchainDbContextFactory
    {
        BlockchainDbContextBase CreateContext();
    }
}