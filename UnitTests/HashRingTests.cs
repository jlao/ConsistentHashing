using ConsistentHashing;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class HashRingTests
    {
        private readonly ITestOutputHelper output;

        public HashRingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AddRemoveNodes()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 10, 210, 310 });
            hashRing.AddNode(2, new uint[] { 20, 220, 320 });
            hashRing.AddNode(3, new uint[] { 30, 230, 330 });

            hashRing.GetNode(15).Should().Be(2);
            hashRing.GetNode(215).Should().Be(2);
            hashRing.GetNode(315).Should().Be(2);

            hashRing.RemoveNode(2);

            hashRing.GetNode(15).Should().Be(3);

            hashRing.GetNode(15).Should().Be(3);
            hashRing.GetNode(215).Should().Be(3);
            hashRing.GetNode(315).Should().Be(3);
        }

        [Fact]
        public void RemoveNonexistentNode()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.RemoveNode(1);

            hashRing.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void GetNodeEmptyRing()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();
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
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            var assignments = hashRing.RangeAssignments.ToArray();
            assignments.Should().HaveCount(0);
        }

        [Fact]
        public void SingleNode()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 100 });

            hashRing.GetNode(10).Should().Be(1);
            hashRing.GetNode(200).Should().Be(1);

            var assignments = hashRing.RangeAssignments.ToArray();

            assignments.Should().HaveCount(1);
            (int node, HashRange range) = assignments[0];
            node.Should().Be(1);
            range.StartExclusive.Should().Be(100);
            range.EndInclusive.Should().Be(100);
        }

        [Fact]
        public void SingleNodeWithManyVirtualNodes()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 100, 200, 300 });

            hashRing.GetNode(10).Should().Be(1);
            hashRing.GetNode(200).Should().Be(1);
            hashRing.GetNode(500).Should().Be(1);
        }

        [Fact]
        public void GetRangeAssignments()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 100, 300, 500 });
            hashRing.AddNode(2, new uint[] { 200, 400, 600 });

            (int node, HashRange range)[] assignments = hashRing.RangeAssignments.ToArray();

            assignments.Should().HaveCount(6);
            AssertAssignment(assignments[0], 2, 100, 200);
            AssertAssignment(assignments[1], 1, 200, 300);
            AssertAssignment(assignments[2], 2, 300, 400);
            AssertAssignment(assignments[3], 1, 400, 500);
            AssertAssignment(assignments[4], 2, 500, 600);
            AssertAssignment(assignments[5], 1, 600, 100);
        }

        [Fact]
        public void GetNodeForHash()
        {
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 100, 300, 500 });
            hashRing.AddNode(2, new uint[] { 200, 400, 600 });

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
            IConsistentHashRing<int> hashRing = new HashRing<int>();

            hashRing.AddNode(1, new uint[] { 100, 300, 500 });
            hashRing.AddNode(2, new uint[] { 200, 400, 600 });

            for (uint i = 301; i <= 400; i++)
            {
                hashRing.GetNode(i).Should().Be(2);
            }
        }

        private static void AssertAssignment(
            (int node, HashRange range) assignment,
            int expectedNode,
            uint expectedStart,
            uint expectedEnd)
        {
            assignment.node.Should().Be(expectedNode);
            assignment.range.StartExclusive.Should().Be(expectedStart);
            assignment.range.EndInclusive.Should().Be(expectedEnd);
        }
    }
}
