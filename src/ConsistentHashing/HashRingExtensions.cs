namespace ConsistentHashing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HashRingExtensions
    {
        /// <summary>
        /// Adds a node to the hash ring many times as virtual nodes.
        /// </summary>
        /// <typeparam name="TNode">The node type.</typeparam>
        /// <param name="ring">The hash ring to add to.</param>
        /// <param name="node">The node to add.</param>
        /// <param name="virtualNodes">The hash points to add the node to.</param>
        public static void AddVirtualNodes<TNode>(this IConsistentHashRing<TNode> ring, TNode node, IEnumerable<uint> virtualNodes)
            where TNode : IComparable<TNode>
        {
            foreach (uint n in virtualNodes)
            {
                ring.AddNode(node, n);
            }
        }

        /// <summary>
        /// Gets all the ranges a node owns as a <see cref="HashRangeCollection"/>.
        /// </summary>
        /// <typeparam name="TNode">The node type.</typeparam>
        /// <param name="ring">The hash ring to get from.</param>
        /// <param name="node">The node to get ranges for.</param>
        /// <returns>A <see cref="HashRangeCollection"/> of all the ranges the specified node owns.</returns>
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
