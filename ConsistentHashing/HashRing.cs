﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ConsistentHashing
{
    public class HashRing<T> : IConsistentHashRing<T>
        where T : IComparable<T>
    {
        private readonly List<RingItem> ring = new List<RingItem>();

        public IEnumerable<Partition<T>> Partitions => this.GetPartitions();

        public bool IsEmpty => this.ring.Count == 0;

        public void AddNode(T node, IEnumerable<uint> virtualNodes)
        {
            foreach (uint virtualNode in virtualNodes)
            {
                var newNode = new RingItem(node, virtualNode);
                int index = this.BinarySearch(virtualNode, true, node);
                if (index < 0)
                {
                    ring.Insert(~index, newNode);
                }
            }
        }

        public IEnumerator<(T, uint)> GetEnumerator()
        {
            foreach (var item in this.ring)
            {
                yield return (item.Node, item.Hash);
            }
        }

        public T GetNode(uint hash)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("Ring is empty");
            }

            int index = this.BinarySearch(hash, false, default(T));
            
            if (index >= 0)
            {
                int prev = index - 1;
                while (prev >= 0 && this.ring[prev].Hash == hash)
                {
                    index = prev;
                    prev--;
                }

                return this.ring[index].Node;
            }
            else
            {
                index = ~index;
                if (index == this.ring.Count)
                {
                    return this.ring[0].Node;
                }
                else
                {
                    return this.ring[index].Node;
                }
            }
        }

        public void RemoveNode(T node)
        {
            bool RemovePredicate(RingItem n)
            {
                return node.CompareTo(n.Node) == 0;
            }

            this.ring.RemoveAll(RemovePredicate);
        }

        private int BinarySearch(uint hash, bool compareNodes, T node)
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
            throw new NotImplementedException();
        }

        private IEnumerable<Partition<T>> GetPartitions()
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
                yield return new Partition<T>(curr.Node, new HashRange(prevHash, curr.Hash));
                prevHash = curr.Hash;
            }

            var last = this.ring[this.ring.Count - 1];
            yield return new Partition<T>(first.Node, new HashRange(last.Hash, first.Hash));
        }

        struct RingItem
        {
            public RingItem(T node, uint hash)
            {
                this.Node = node;
                this.Hash = hash;
            }

            public T Node { get; }

            public uint Hash { get; }

            public override string ToString()
            {
                return $"{this.Node} - {this.Hash}";
            }
        }
    }
}
