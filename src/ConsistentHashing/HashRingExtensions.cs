namespace ConsistentHashing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public static HashRangeCollection GetRangesForNode<TNode>(this IConsistentHashRing<TNode> ring, TNode node)
            where TNode : IComparable<TNode>
        {
            return new HashRangeCollection(
                ring.Partitions
                    .Where(p => p.Node.CompareTo(node) == 0)
                    .Select(p => p.Range));
        }
    }
}
