namespace ConsistentHashing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a consistent hash ring.
    /// </summary>
    /// <typeparam name="TNode">The type of node to store in the ring.</typeparam>
    public interface IConsistentHashRing<TNode> : IEnumerable<(TNode, uint)>
        where TNode : IComparable<TNode>
    {
        /// <summary>
        /// Gets all partitions where a partition is a hash range and the owner node.
        /// </summary>
        /// <value>An enumeration of all the partitions defined by the hash ring.</value>
        IEnumerable<Partition<TNode>> Partitions { get; }

        /// <summary>
        /// Gets whether the consistent hash ring is empty or not.
        /// </summary>
        /// <value>True if the ring is empty and false otherwise.</value>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds the specified node with virtual nodes.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="virtualNodes">One or more points on the hash ring to add the node to.</param>
        void AddNode(TNode node, IEnumerable<uint> virtualNodes);

        /// <summary>
        /// Removes all virtual nodes for the specified node from the hash ring.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        void RemoveNode(TNode node);

        /// <summary>
        /// Gets the node that owns the hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>The node that owns the hash.</returns>
        TNode GetNode(uint hash);
    }
}
