namespace ConsistentHashing
{
    /// <summary>
    /// Represents a range of hashes in the consistent hash ring.
    /// </summary>
    public struct HashRange
    {
        /// <summary>
        /// Creates a hash range for (startExclusive, endInclusive].
        /// </summary>
        /// <param name="startExclusive">The start of the range (exclusive).</param>
        /// <param name="endInclusive">The end of the range (inclusive).</param>
        public HashRange(uint startExclusive, uint endInclusive)
        {
            this.StartExclusive = startExclusive;
            this.EndInclusive = endInclusive;
        }

        /// <summary>
        /// Gets the exclusive start of the hash range.
        /// </summary>
        public uint StartExclusive { get; }

        /// <summary>
        /// Gets the inclusive end of the hash range.
        /// </summary>
        public uint EndInclusive { get; }

        /// <summary>
        /// Checks whether the specified hash is contained in the range.
        /// </summary>
        /// <param name="hash">The hash to check.</param>
        /// <returns>True if the hash is in the range and false otherwise.</returns>
        public bool Contains(uint hash)
        {
            return hash > this.StartExclusive && hash <= this.EndInclusive;
        }
    }
}
