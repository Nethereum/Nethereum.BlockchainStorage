using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class StaticBlockRangeProgressService : BlockProgressServiceBase
    {
        public StaticBlockRangeProgressService(
            BigInteger blockNumberFrom,
            BigInteger blockNumberTo,
            IBlockProgressRepository blockProgressRepository) : 
            base(
                blockNumberFrom, 
                blockProgressRepository)
        {
            BlockNumberTo = blockNumberTo;
        }

        public BigInteger BlockNumberTo { get; }

        public override Task<BigInteger> GetBlockNumberToProcessTo()
        {
            return Task.FromResult((BigInteger)BlockNumberTo);
        }
    }
}