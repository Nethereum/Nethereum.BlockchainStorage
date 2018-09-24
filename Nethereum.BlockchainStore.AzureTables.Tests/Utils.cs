using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.AzureTables.Tests
{
    public static class Utils
    {
        public static long ToLong(this HexBigInteger val)
        {
            return (long) val.Value;
        }
    }
}
