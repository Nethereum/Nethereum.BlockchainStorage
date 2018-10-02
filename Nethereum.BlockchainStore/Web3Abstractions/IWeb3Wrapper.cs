namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IWeb3Wrapper:         
        IGetBlockWithTransactionHashesByNumber,
        ITransactionProxy,
        IGetCode,
        IGetTransactionVMStack
    {

    }
}
