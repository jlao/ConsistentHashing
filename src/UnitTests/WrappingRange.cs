namespace UnitTests
{
    using ConsistentHashing;
    using FluentAssertions;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class WrappingRange : IEnumerable<uint>
    {
        private readonly uint startExclusive;
        private readonly uint endInclusive;

        public WrappingRange(HashRange range) : this(range.StartExclusive, range.EndInclusive)
        {
        }

        public WrappingRange(uint startExclusive, uint endInclusive)
        {
            this.startExclusive = startExclusive;
            this.endInclusive = endInclusive;
        }

        public IEnumerator<uint> GetEnumerator()
        {
            uint i = startExclusive + 1;
            while (i != this.endInclusive + 1)
            {
                yield return i;
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
