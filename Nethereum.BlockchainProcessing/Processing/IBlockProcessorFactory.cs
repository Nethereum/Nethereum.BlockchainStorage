using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProcessorFactory
    {
        IBlockProcessor Create(
            IWeb3Wrapper web3, 
            IVmStackErrorChecker vmStackErrorChecker, 
            HandlerContainer handlers,
            FilterContainer filters,
            bool postVm = false, 
            bool processTransactionsInParallel = false);

        IBlockProcessor Create(
            IWeb3Wrapper web3, 
            HandlerContainer handlers,
            FilterContainer filters,
            bool postVm = false, 
            bool processTransactionsInParallel = false);
    }
}
