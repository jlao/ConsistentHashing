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
        /// Adds the specified node to the hash ring at the specified point.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="point">The point at which to add the node to.</param>
        void AddNode(TNode node, uint point);

        /// <summary>
        /// Removes all instances of the node from the hash ring.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        void RemoveNode(TNode node);

        /// <summary>
        /// Gets the node that owns the hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>The node that owns the hash.</returns>
        TNode GetNode(uint hash);

        /// <summary>
        /// Gets the node that owns the hash, and the next n - 1 unique nodes
        /// in the ring.  If a node appears on the ring multiple times as
        /// virtual nodes, only a single instance will be be returned and count
        /// toward the limit.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="n">How many nodes to return.  May be less than n if n is greater than the number of nodes in the ring.</param>
        /// <returns>The node that owns the hash.</returns>
        List<TNode> GetNodes(uint hash, int n);
    }
}
