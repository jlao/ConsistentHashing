# Internals

`HashRing<TNode>` is implemented as a sorted list sorted by points on the ring for the nodes.

## Complexity

* `IsEmpty` is O(1)
* `AddNode` is O(n)
* `GetNode` is O(log n)
* `RemoveNode` is O(n)
* `GetPartitions` is O(n)
* `GetEnumerator` is O(n)

## Thread safety

`HashRing<TNode>` is not thread safe.