using System;
using System.Numerics;
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
            BigInteger? defaultStartingBlockNumber,
            IBlockProgressRepository blockProgressRepository,
            uint? minimumBlockConfirmations = null) :
            this(web3.Eth.Blocks, defaultStartingBlockNumber, blockProgressRepository, minimumBlockConfirmations)
        {
        }

        public BlockProgressService(
            IEthApiBlockService blockService,
            BigInteger? defaultStartingBlockNumber, 
            IBlockProgressRepository blockProgressRepository,
            uint? minimumBlockConfirmations = null) : 
            base(
            defaultStartingBlockNumber, 
            blockProgressRepository)
        {
            _web3 = blockService;
            MinimumBlockConfirmations = minimumBlockConfirmations ?? 0;
        }

        protected override Task<BigInteger> GetMinBlockNumber() => GetCurrentBlockNumberLessMinimumConfirmations();

        public override Task<BigInteger> GetBlockNumberToProcessTo() => GetCurrentBlockNumberLessMinimumConfirmations();

        private  async Task<BigInteger> GetCurrentBlockNumberLessMinimumConfirmations()
        {
            return (await _web3.GetBlockNumber.SendRequestAsync()
                       .ConfigureAwait(false)).ToUlong() - MinimumBlockConfirmations;
        }
    }
}