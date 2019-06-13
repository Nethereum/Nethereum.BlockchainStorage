using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.Services;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockProgressService : BlockProgressServiceBase
    {
        private readonly IEthApiBlockService _web3;

        public uint MinimumBlockConfirmations { get; }

        public BlockProgressService(
            IWeb3 web3,
            ulong? defaultStartingBlockNumber,
            IBlockProgressRepository blockProgressRepository,
            uint? minimumBlockConfirmations = null) :
            this(web3.Eth.Blocks, defaultStartingBlockNumber, blockProgressRepository, minimumBlockConfirmations)
        {
        }

        public BlockProgressService(
            IEthApiBlockService blockService, 
            ulong? defaultStartingBlockNumber, 
            IBlockProgressRepository blockProgressRepository,
            uint? minimumBlockConfirmations = null) : 
            base(
            defaultStartingBlockNumber, 
            blockProgressRepository)
        {
            _web3 = blockService;
            MinimumBlockConfirmations = minimumBlockConfirmations ?? 0;
        }

        protected override Task<ulong> GetMinBlockNumber() => GetCurrentBlockNumberLessMinimumConfirmations();

        public override Task<ulong> GetBlockNumberToProcessTo() => GetCurrentBlockNumberLessMinimumConfirmations();

        private  async Task<ulong> GetCurrentBlockNumberLessMinimumConfirmations()
        {
            return (await _web3.GetBlockNumber.SendRequestAsync()
                       .ConfigureAwait(false)).ToUlong() - MinimumBlockConfirmations;
        }
    }
}