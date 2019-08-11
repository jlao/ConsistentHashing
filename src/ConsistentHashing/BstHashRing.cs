using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsistentHashing
{
    public class BstHashRing<TNode> : IConsistentHashRing<TNode>
        where TNode : IComparable<TNode>
    {
        private TreeNode root;

        public BstHashRing()
        {
        }

        public IEnumerable<Partition<TNode>> Partitions =>
            EnumerateRangeAssignments(this.root);

        public bool IsEmpty => this.root == null;

        public void AddNode(TNode node, IEnumerable<uint> virtualNodes)
        {
            foreach (uint virtualNode in virtualNodes)
            {
                AddNode(ref this.root, new TreeNode(node, virtualNode));
            }
        }

        public void RemoveNode(TNode node)
        {
            throw new NotImplementedException();
        }

        public TNode GetNode(uint hash)
        {
            if (this.root == null)
            {
                throw new InvalidOperationException("Hash ring is empty");
            }

            TreeNode n = GetNode(this.root);
            return n == null ? GetMin(this.root).Node : n.Node;

            TreeNode GetNode(TreeNode root)
            {
                TreeNode result = null;

                while (root != null)
                {
                    if (root.HashValue > hash)
                    {
                        result = root;
                        root = root.Left;
                    }
                    else if (root.HashValue < hash)
                    {
                        root = root.Right;
                    }
                    else
                    {
                        return root;
                    }
                }

                return result;
            }
        }

        private static TreeNode GetMin(TreeNode root)
        {
            if (root == null)
            {
                return null;
            }

            while (root.Left != null)
            {
                root = root.Left;
            }

            return root;
        }

        private static IEnumerable<Partition<TNode>> EnumerateRangeAssignments(TreeNode root)
        {
            if (root == null)
            {
                yield break;
            }

            foreach ((TreeNode first, TreeNode second) in Pairs(InOrderTraversal(root)))
            {
                yield return new Partition<TNode>(second.Node, new HashRange(first.HashValue, second.HashValue));
            }
        }

        private static IEnumerable<(TreeNode, TreeNode)> Pairs(IEnumerable<TreeNode> nodes)
        {
            TreeNode first = null;
            TreeNode curr = null;

            foreach (TreeNode n in nodes)
            {
                if (first == null)
                {
                    first = n;
                }

                if (curr == null)
                {
                    curr = n;
                    continue;
                }

                yield return (curr, n);
                curr = n;
            }

            yield return (curr, first);
        }

        private static IEnumerable<TreeNode> InOrderTraversal(TreeNode root)
        {
            if (root == null)
            {
                yield break;
            }

            // Invariant: all items in the stack except
            // for the top item are non-null. The top item
            // _may_ be null.
            Stack<TreeNode> stack = new Stack<TreeNode>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var n = stack.Peek();

                if (n == null)
                {
                    // Pop the null element off
                    stack.Pop();

                    if (stack.Count == 0)
                    {
                        break;
                    }

                    // This is non-null
                    n = stack.Pop();
                    yield return n;
                    stack.Push(n.Right);
                    continue;
                }

                stack.Push(n.Left);
            }
        }

        private static void AddNode(ref TreeNode root, TreeNode newNode)
        {
            if (root == null)
            {
                root = newNode;
                return;
            }

            TreeNode pointer = root;

            while (pointer != null)
            {
                if (pointer.HashValue > newNode.HashValue)
                {
                    if (pointer.Left != null)
                    {
                        pointer = pointer.Left;
                    }
                    else
                    {
                        pointer.Left = newNode;
                        return;
                    }
                }
                else if (pointer.HashValue < newNode.HashValue)
                {
                    if (pointer.Right != null)
                    {
                        pointer = pointer.Right;
                    }
                    else
                    {
                        pointer.Right = newNode;
                        return;
                    }
                }
                else
                {
                    throw new ArgumentException("Two nodes with same hash value");
                }
            }
        }

        public IEnumerator<(TNode, uint)> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private class TreeNode
        {
            public TreeNode(TNode node, uint hashValue)
            {
                this.Node = node;
                this.HashValue = hashValue;
            }

            public TNode Node { get; }

            public uint HashValue { get; }

            public TreeNode Left { get; set; }

            public TreeNode Right { get; set; }

            public override string ToString()
            {
                return $"{this.Node} - {this.HashValue}";
            }
        }
    }
}
