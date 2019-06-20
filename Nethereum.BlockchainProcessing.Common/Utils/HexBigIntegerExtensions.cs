using Nethereum.Hex.HexTypes;
using System.Numerics;

namespace System
{
    public static class HexBigIntegerExtension
    {
        public static HexBigInteger ToHexBigInteger(this ulong val)
        {
            return new HexBigInteger(val);
        }

        public static HexBigInteger ToHexBigInteger(this int val)
        {
            return new HexBigInteger(val);
        }

        public static HexBigInteger ToHexBigInteger(this BigInteger val)
        {
            return new HexBigInteger(val);
        }

        public static ulong ToUlong(this HexBigInteger val)
        {
            return (ulong)val.Value;
        }
    }
}
