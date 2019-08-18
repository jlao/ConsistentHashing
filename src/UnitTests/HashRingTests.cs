using ConsistentHashing;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
#if false
    public class BstHashRingTests : HashRingTestBase
    {
        protected override IConsistentHashRing<int> CreateRing()
        {
            return new BstHashRing<int>();
        }
    }
#endif

    public class HashRingTests : HashRingTestBase
    {
        protected override IConsistentHashRing<int> CreateRing()
        {
            return new HashRing<int>();
        }
    }

    public abstract class HashRingTestBase
    {
        protected abstract IConsistentHashRing<int> CreateRing();

        [Fact]
        public void AddRemoveNodes()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(2, new uint[] { 20, 220, 320 });
            hashRing.AddVirtualNodes(1, new uint[] { 10, 210, 310 });
            hashRing.AddVirtualNodes(3, new uint[] { 30, 230, 330 });

            var expected = new (int, uint)[]
            {
                (1, 10),
                (2, 20),
                (3, 30),

                (1, 210),
                (2, 220),
                (3, 230),

                (1, 310),
                (2, 320),
                (3, 330),
            };

            hashRing.ToList().Should().Equal(expected);

            hashRing.RemoveNode(2);

            expected = new (int, uint)[]
            {
                (1, 10),
                (3, 30),

                (1, 210),
                (3, 230),

                (1, 310),
                (3, 330),
            };

            hashRing.ToList().Should().Equal(expected);
        }

        [Fact]
        public void AddVirtualNodessSameHash()
        {
            var hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(2, new uint[] { 10, 20, 30 });
            hashRing.AddVirtualNodes(1, new uint[] { 10, 20, 30 });
            hashRing.AddVirtualNodes(3, new uint[] { 10, 20, 30 });

            var expected = new (int, uint)[]
            {
                (1, 10),
                (2, 10),
                (3, 10),

                (1, 20),
                (2, 20),
                (3, 20),

                (1, 30),
                (2, 30),
                (3, 30),
            };

            hashRing.ToList().Should().Equal(expected);
        }

        [Fact]
        public void GetNodeReturnsLowestNodeWhenSameHash()
        {
            var hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(2, new uint[] { 10, 20, 30 });
            hashRing.AddVirtualNodes(1, new uint[] { 10, 20, 30 });
            hashRing.AddVirtualNodes(3, new uint[] { 10, 20, 30 });

            hashRing.AddVirtualNodes(4, new uint[] { 15, 25, 35 });

            hashRing.GetNode(12).Should().Be(4);
            hashRing.GetNode(16).Should().Be(1);
            hashRing.GetNode(20).Should().Be(1);
            hashRing.GetNode(27).Should().Be(1);
            hashRing.GetNode(40).Should().Be(1);
        }

        [Fact]
        public void RemoveNonexistentNode()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.RemoveNode(1);

            hashRing.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void GetNodeEmptyRing()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();
            hashRing.IsEmpty.Should().BeTrue();

            Action action = () =>
            {
                hashRing.GetNode(10);
            };

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetRangeAssignmentsEmptyRing()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            var partitions = hashRing.Partitions.ToArray();
            partitions.Should().HaveCount(0);
        }

        [Fact]
        public void SingleNode()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100 });

            hashRing.GetNode(10).Should().Be(1);
            hashRing.GetNode(200).Should().Be(1);

            var partitions = hashRing.Partitions.ToArray();

            partitions.Should().HaveCount(1);
            Partition<int> partition = partitions[0];
            partition.Node.Should().Be(1);
            partition.Range.StartExclusive.Should().Be(100);
            partition.Range.EndInclusive.Should().Be(100);
        }

        [Fact]
        public void SingleNodeWithManyVirtualNodes()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100, 200, 300 });

            hashRing.GetNode(10).Should().Be(1);
            hashRing.GetNode(200).Should().Be(1);
            hashRing.GetNode(500).Should().Be(1);
        }

        [Fact]
        public void GetRangeAssignments()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100, 300, 500 });
            hashRing.AddVirtualNodes(2, new uint[] { 200, 400, 600 });

            Partition<int>[] partitions = hashRing.Partitions.ToArray();

            partitions.Should().HaveCount(6);
            AssertPartition(partitions[0], 2, 100, 200);
            AssertPartition(partitions[1], 1, 200, 300);
            AssertPartition(partitions[2], 2, 300, 400);
            AssertPartition(partitions[3], 1, 400, 500);
            AssertPartition(partitions[4], 2, 500, 600);
            AssertPartition(partitions[5], 1, 600, 100);
        }

        [Fact]
        public void GetNodeForHash()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100, 300, 500 });
            hashRing.AddVirtualNodes(2, new uint[] { 200, 400, 600 });

            hashRing.GetNode(101).Should().Be(2);
            hashRing.GetNode(200).Should().Be(2);
            hashRing.GetNode(201).Should().Be(1);
            hashRing.GetNode(300).Should().Be(1);
            hashRing.GetNode(301).Should().Be(2);
            hashRing.GetNode(400).Should().Be(2);
            hashRing.GetNode(401).Should().Be(1);
            hashRing.GetNode(500).Should().Be(1);
            hashRing.GetNode(501).Should().Be(2);
            hashRing.GetNode(600).Should().Be(2);
            hashRing.GetNode(601).Should().Be(1);
            hashRing.GetNode(100).Should().Be(1);
        }

        [Fact]
        public void VerifyAllHashesInRange()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100, 300, 500 });
            hashRing.AddVirtualNodes(2, new uint[] { 200, 400, 600 });

            for (uint i = 301; i <= 400; i++)
            {
                hashRing.GetNode(i).Should().Be(2);
            }
        }

        [Fact]
        public void GetRangesForNode()
        {
            IConsistentHashRing<int> hashRing = this.CreateRing();

            hashRing.AddVirtualNodes(1, new uint[] { 100, 300, 500 });
            hashRing.AddVirtualNodes(2, new uint[] { 200, 400, 600 });

            hashRing.GetRangesForNode(1).Should().BeEquivalentTo(
                new HashRange(200, 300),
                new HashRange(400, 500),
                new HashRange(600, 100));

            hashRing.GetRangesForNode(2).Should().BeEquivalentTo(
                new HashRange(100, 200),
                new HashRange(300, 400),
                new HashRange(500, 600));
        }

        private static void AssertPartition(
            Partition<int> partition,
            int expectedNode,
            uint expectedStart,
            uint expectedEnd)
        {
            partition.Node.Should().Be(expectedNode);
            partition.Range.StartExclusive.Should().Be(expectedStart);
            partition.Range.EndInclusive.Should().Be(expectedEnd);
        }
    }
}
