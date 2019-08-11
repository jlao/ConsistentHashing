﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentHashing
{
    public interface IConsistentHashRing<TNode> : IEnumerable<(TNode, uint)>
    {
        IEnumerable<(TNode, HashRange)> RangeAssignments { get; }

        bool IsEmpty { get; }

        void AddNode(TNode node, IEnumerable<uint> virtualNodes);

        void RemoveNode(TNode node);

        TNode GetNode(uint hash);
    }
}
