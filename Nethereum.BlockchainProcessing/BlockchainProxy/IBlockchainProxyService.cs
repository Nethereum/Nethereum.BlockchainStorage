namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IBlockchainProxyService:         
        IBlockProxy,
        ITransactionProxy,
        IGetCode,
        IGetTransactionVMStack,
        IEventLogProxy
    {

    }
}
