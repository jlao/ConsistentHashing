using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentHashing
{
    public interface IConsistentHashRing<TNode>
    {
        void AddNode(TNode node, IEnumerable<uint> virtualNodes);

        IEnumerable<(TNode, HashRange)> RangeAssignments { get; }

        TNode GetNode(uint hash);
    }
}
