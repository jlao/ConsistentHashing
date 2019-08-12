using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentHashing
{
    public static class HashRingExtensions
    {
        public static void AddVirtualNodes<TNode>(this IConsistentHashRing<TNode> ring, TNode node, IEnumerable<uint> virtualNodes)
            where TNode : IComparable<TNode>
        {
            foreach (uint n in virtualNodes)
            {
                ring.AddNode(node, n);
            }
        }
    }
}
