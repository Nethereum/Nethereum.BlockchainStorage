using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.Entities.Mapping
{
    public static class Utils
    {
        public static long ToLong(this HexBigInteger val)
        {
            return val == null ? 0 : (long) val.Value;
        }
    }
}
