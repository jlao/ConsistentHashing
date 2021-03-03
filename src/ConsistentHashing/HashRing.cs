using System.Runtime.InteropServices;

namespace ConsistentHashing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a consistent hash ring.
    /// </summary>
    /// <typeparam name="TNode">The type of node to store in the ring.</typeparam>
    public class HashRing<TNode> : IConsistentHashRing<TNode>
        where TNode : IComparable<TNode>
    {
        private readonly List<RingItem> ring = new List<RingItem>();

        /// <summary>
        /// Gets all partitions where a partition is a hash range and the owner node.
        /// </summary>
        /// <value>An enumeration of all the partitions defined by the hash ring.</value>
        public IEnumerable<Partition<TNode>> Partitions => this.GetPartitions();

        /// <summary>
        /// Gets whether the consistent hash ring is empty or not.
        /// </summary>
        /// <value>True if the ring is empty and false otherwise.</value>
        public bool IsEmpty => this.ring.Count == 0;

        /// <summary>
        /// Adds the specified node to the hash ring at the specified point.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="point">The point at which to add the node to.</param>
        public void AddNode(TNode node, uint point)
        {
            var newNode = new RingItem(node, point);
            int index = this.BinarySearch(point, true, node);
            if (index < 0)
            {
                ring.Insert(~index, newNode);
            }
        }

        /// <inheritdoc />
        public IEnumerator<(TNode, uint)> GetEnumerator()
        {
            foreach (var item in this.ring)
            {
                yield return (item.Node, item.Hash);
            }
        }

        /// <summary>
        /// Gets the node that owns the hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>The node that owns the hash.</returns>
        public TNode GetNode(uint hash)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("Ring is empty");
            }

            int index = this.GetNodeIndex(hash);

            return this.ring[index].Node;
        }


        /// <summary>
        /// Gets the node that owns the hash, and the next n - 1 nodes in the ring.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="n">How many nodes to return.  May be less than n if n is greater than the number of nodes in the ring.</param>
        /// <returns>The node that owns the hash.</returns>
        public List<TNode> GetNodes(uint hash, int n)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("Ring is empty");
            }

            if (n < 1)
            {
                throw new InvalidOperationException(
                    $"GetNodes() parameter n must be greater or equal to 1, but it was {n}");
            }

            var nodes = new List<TNode>();

            int curIndex = this.GetNodeIndex(hash);
            n = Math.Min(n, ring.Count);
            while (n-- > 0)
            {
                nodes.Add(ring[curIndex].Node);

                if (++curIndex == ring.Count)
                {
                    curIndex = 0;
                }
            }

            return nodes;
        }


        /// <summary>
        /// Removes all instances of the node from the hash ring.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveNode(TNode node)
        {
            bool RemovePredicate(RingItem n)
            {
                return node.CompareTo(n.Node) == 0;
            }

            this.ring.RemoveAll(RemovePredicate);
        }

        private int BinarySearch(uint hash, bool compareNodes, TNode node)
        {
            int start = 0;
            int end = ring.Count - 1;

            while (start <= end)
            {
                int mid = start + ((end - start) / 2);
                uint midHash = ring[mid].Hash;

                if (midHash == hash)
                {
                    if (compareNodes)
                    {
                        int nodeComparison = node.CompareTo(ring[mid].Node);
                        if (nodeComparison == 0)
                        {
                            return mid;
                        }
                        else if (nodeComparison < 0)
                        {
                            end = mid - 1;
                        }
                        else
                        {
                            start = mid + 1;
                        }
                    }
                    else
                    {
                        return mid;
                    }
                }
                else if (midHash > hash)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return ~start;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<Partition<TNode>> GetPartitions()
        {
            if (this.IsEmpty)
            {
                yield break;
            }

            var first = this.ring[0];
            uint prevHash = first.Hash;

            for (int i = 1; i < this.ring.Count; i++)
            {
                var curr = this.ring[i];
                yield return new Partition<TNode>(curr.Node, new HashRange(prevHash, curr.Hash));
                prevHash = curr.Hash;
            }

            var last = this.ring[this.ring.Count - 1];
            yield return new Partition<TNode>(first.Node, new HashRange(last.Hash, first.Hash));
        }

        private int GetNodeIndex(uint hash)
        {
            int index = this.BinarySearch(hash, false, default(TNode));
            
            if (index >= 0)
            {
                int prev = index - 1;
                while (prev >= 0 && this.ring[prev].Hash == hash)
                {
                    index = prev;
                    prev--;
                }
            }
            else
            {
                index = ~index;
                if (index == this.ring.Count)
                {
                    index = 0;
                }
            }

            return index;
        }

        struct RingItem
        {
            public RingItem(TNode node, uint hash)
            {
                this.Node = node;
                this.Hash = hash;
            }

            public TNode Node { get; }

            public uint Hash { get; }

            public override string ToString()
            {
                return $"{this.Node} - {this.Hash}";
            }
        }
    }
}
