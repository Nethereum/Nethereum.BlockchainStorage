namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IWeb3Wrapper:         
        IBlockProxy,
        ITransactionProxy,
        IGetCode,
        IGetTransactionVMStack
    {

    }
}
