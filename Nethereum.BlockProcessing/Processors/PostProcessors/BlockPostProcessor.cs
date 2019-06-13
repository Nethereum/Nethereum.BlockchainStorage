using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.Services;
using Nethereum.Web3;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors.PostProcessors
{
    public class BlockPostProcessor : BlockProcessor
    {
        public BlockPostProcessor(IWeb3 web3, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) 
            : base(web3, blockHandler, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(BigInteger blockNumber)
        {
            var block = await BlockProxy.Blocks
                .GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger())
                .ConfigureAwait(false);
            //no need to save the block again
            await ProcessTransactions(block).ConfigureAwait(false);
        }
    }
}