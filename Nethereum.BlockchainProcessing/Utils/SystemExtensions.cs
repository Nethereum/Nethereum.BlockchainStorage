using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class SystemExtensions
    {
        public static HexBigInteger ToHexBigInteger(this ulong val)
        {
            return new HexBigInteger(val);
        }

        public static HexBigInteger ToHexBigInteger(this int val)
        {
            return new HexBigInteger(val);
        }

        public static ulong ToUlong(this HexBigInteger val)
        {
            return (ulong)val.Value;
        }
    }
}
