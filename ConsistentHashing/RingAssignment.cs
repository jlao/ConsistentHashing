using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsistentHashing
{
    public struct RingAssignment
    {
        public RingAssignment(IEnumerable<HashRange> ranges)
        {
            this.Ranges = ranges.ToList();
        }

        public IReadOnlyList<HashRange> Ranges { get; }
    }

    public struct HashRange
    {
        public HashRange(uint start, uint end)
        {
            this.StartExclusive = start;
            this.EndInclusive = end;
        }

        public uint StartExclusive { get; }

        public uint EndInclusive { get; }
    }
}
