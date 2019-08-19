# Internals

[`HashRing<TNode>`](xref:ConsistentHashing.HashRing`1) is implemented as a sorted list sorted by points on the ring for the nodes.

## Complexity

* [`IsEmpty`](xref:ConsistentHashing.HashRing`1.IsEmpty) is O(1)
* [`AddNode`](xref:ConsistentHashing.HashRing`1.AddNode(`0,System.UInt32)) is O(n)
* [`GetNode`](xref:ConsistentHashing.HashRing`1.GetNode(System.UInt32)) is O(log n)
* [`RemoveNode`](xref:ConsistentHashing.HashRing`1.RemoveNode(`0)) is O(n)
* [`Partitions`](xref:ConsistentHashing.HashRing`1.Partitions) is O(n)
* `GetEnumerator` is O(n)

## Thread safety

[`HashRing<TNode>`](xref:ConsistentHashing.HashRing`1) is not thread safe.