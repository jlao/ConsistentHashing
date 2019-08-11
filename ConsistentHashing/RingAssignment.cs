using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsistentHashing
{
    public struct Partition<T>
    {
        public Partition(T node, HashRange range)
        {
            this.Node = node;
            this.Range = range;
        }

        public T Node { get; }

        public HashRange Range { get; }
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
