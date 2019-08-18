# ConsistentHashing

This is an implementation of [consistent hashing](https://en.wikipedia.org/wiki/Consistent_hashing) for .NET.

## Getting Started

Add a using statement:

```csharp
using ConsistentHashing;
```

The primary class is `HashRing<TNode>` which implements `IConsistentHashRing<TNode>`.
We'll start by creating a hash ring and adding a few nodes. We use `string` for our
node class, but it can be anything that implements `IComparable<TNode>`.

```csharp
var hashRing = new HashRing<string>();

hashRing.AddNode("a", 100);
hashRing.AddNode("b", 200);
hashRing.AddNode("a", 250);
hashRing.AddNode("c", 300);
```

This creates a consistent hash ring with the following ownership ranges:

* a: (200, 250], (300, 100]
* b: (100, 200]
* c: (250, 300]

Note that you can add the same node multiple times and it will own multiple ranges.
Node a's second range, (300, 100] wraps around so it covers 301 to `uint.MaxValue` and
0 to 100.

Now we can do ownership checks:

```csharp
Assert.Equal("a", hashRing.GetNode(0));
Assert.Equal("a", hashRing.GetNode(100));
Assert.Equal("a", hashRing.GetNode(500));
Assert.Equal("b", hashRing.GetNode(200));
Assert.Equal("a", hashRing.GetNode(225));
Assert.Equal("c", hashRing.GetNode(300));
Assert.Equal("a", hashRing.GetNode(400));
```

We can enumerate all the partitions:

```csharp
foreach (Partition<string> p in hashRing.Partitions)
{
    Console.WriteLine($"{p.Node}: ({p.Range.StartExclusive}, {p.Range.EndInclusive}]");
}
```

Which will output:

```
b: (100, 200]
a: (200, 250]
c: (250, 300]
a: (300, 100]
```