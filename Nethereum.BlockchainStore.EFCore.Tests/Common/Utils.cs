using System;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.EFCore.Tests.Common
{
    public static class Utils
    {
        public static HexBigInteger CreateBlockTimestamp()
        {
            return new HexBigInteger(DateTimeOffset.UnixEpoch.ToUnixTimeSeconds());
        }
    }
}
