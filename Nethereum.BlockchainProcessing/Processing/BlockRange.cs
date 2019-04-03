using System;

namespace Nethereum.BlockchainProcessing.Processing
{
    public struct BlockRange: 
        IEquatable<BlockRange>, 
        IEquatable<(ulong From, ulong To)>
    {
        private readonly int _hashCode;

        public BlockRange(ulong from, ulong to)
        {
            From = from;
            To = to;
            BlockCount = (To - From) + 1;
            _hashCode = new {From, To}.GetHashCode();
        }

        public ulong From { get;  }
        public ulong To { get; }
        public ulong BlockCount { get; }

        public bool Equals(BlockRange other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object obj)
        {
            if (obj is BlockRange other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals((ulong From, ulong To) other)
        {
            return To.Equals(other.From) && From.Equals(other.To);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}