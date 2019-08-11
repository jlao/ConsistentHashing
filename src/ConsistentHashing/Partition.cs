namespace ConsistentHashing
{
    /// <summary>
    /// Represents a node and a range on the consistent hash ring that it owns.
    /// </summary>
    /// <typeparam name="TNode">The type of the node.</typeparam>
    public struct Partition<TNode>
    {
        /// <summary>
        /// Creates a new partition with the specified node and hash range.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="range">The range.</param>
        public Partition(TNode node, HashRange range)
        {
            this.Node = node;
            this.Range = range;
        }

        /// <summary>
        /// Gets the node that owns <see cref="Range"/>.
        /// </summary>
        public TNode Node { get; }

        /// <summary>
        /// Gets the range of hashes owned by <see cref="Node"/>.
        /// </summary>
        public HashRange Range { get; }
    }
}
