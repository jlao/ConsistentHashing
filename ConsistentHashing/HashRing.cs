using System;
using System.Collections.Generic;

namespace ConsistentHashing
{
    public class HashRing<TNode> : IConsistentHashRing<TNode>
    {
        private TreeNode root;
        private readonly uint numVirtualNodes;
        private readonly Random rng;

        public HashRing(uint numVirtualNodes)
        {
            this.numVirtualNodes = numVirtualNodes;
            this.rng = new Random();
        }

        public IEnumerable<(TNode, HashRange)> RangeAssignments =>
            EnumerateRangeAssignments(this.root);

        public void AddNode(TNode node)
        {
            for (uint i = 0; i < this.numVirtualNodes; i++)
            {
                var newNode = new TreeNode(node, (uint)this.rng.Next());
                AddNode(ref this.root, newNode);
            }
        }

        public void AddNode(TNode node, IEnumerable<uint> virtualNodes)
        {
            foreach (uint virtualNode in virtualNodes)
            {
                AddNode(ref this.root, new TreeNode(node, virtualNode));
            }
        }

        public TNode GetNode(uint hash)
        {
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

        private static IEnumerable<(TNode, HashRange)> EnumerateRangeAssignments(TreeNode root)
        {
            if (root == null)
            {
                yield break;
            }

            foreach ((TreeNode first, TreeNode second) in Pairs(InOrderTraversal(root)))
            {
                yield return (second.Node, new HashRange(first.HashValue, second.HashValue));
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
