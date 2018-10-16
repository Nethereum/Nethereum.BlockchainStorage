namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IWeb3Wrapper:         
        IBlockProxy,
        ITransactionProxy,
        IGetCode,
        IGetTransactionVMStack
    {

    }
}
