using System;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.EF.Tests.Base.Common
{
    public static class Utils
    {
        public static HexBigInteger CreateBlockTimestamp()
        {
            return new HexBigInteger((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}
