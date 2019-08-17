namespace UnitTests
{
    using ConsistentHashing;
    using FluentAssertions;
    using Xunit;

    public class HashRangeCollectionTests
    {
        [Fact]
        public void ContainsTests()
        {
            var coll = new HashRangeCollection(
                new HashRange[]
                {
                    new HashRange(10, 20),
                    new HashRange(30, 40),
                    new HashRange(50, 60),
                });

            for (uint i = 0; i <= 10; i++)
            {
                coll.Contains(i).Should().BeFalse();
            }

            for (uint i = 11; i <= 20; i++)
            {
                coll.Contains(i).Should().BeTrue($"{i} in (10, 20]");
            }

            for (uint i = 21; i <= 30; i++)
            {
                coll.Contains(i).Should().BeFalse();
            }

            for (uint i = 31; i <= 40; i++)
            {
                coll.Contains(i).Should().BeTrue();
            }

            for (uint i = 41; i <= 50; i++)
            {
                coll.Contains(i).Should().BeFalse();
            }

            for (uint i = 51; i <= 60; i++)
            {
                coll.Contains(i).Should().BeTrue();
            }

            for (uint i = 61; i <= 70; i++)
            {
                coll.Contains(i).Should().BeFalse();
            }
        }

        [Fact]
        public void WrapAroundRange()
        {
            var wrapAround = new HashRange(uint.MaxValue - 10, 5);

            var coll = new HashRangeCollection(
                new HashRange[]
                {
                    new HashRange(10, 20),
                    wrapAround,
                });

            for (uint i = 11; i <= 20; i++)
            {
                coll.Contains(i).Should().BeTrue();
            }

            foreach (uint i in new WrappingRange(wrapAround))
            {
                coll.Contains(i).Should().BeTrue($"{i} in ({uint.MaxValue - 10}, 5]");
            }
        }
    }
}
