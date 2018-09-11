﻿using System;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public static class Utils
    {
        public static HexBigInteger CreateBlockTimestamp()
        {
            return new HexBigInteger(DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
}
