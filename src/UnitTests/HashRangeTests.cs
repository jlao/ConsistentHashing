namespace UnitTests
{
    using ConsistentHashing;
    using FluentAssertions;
    using Xunit;

    public class HashRangeTests
    {
        [Fact]
        public void ContainsTest()
        {
            var range = new HashRange(10, 15);

            for (uint i = 11; i <= 15; i++)
            {
                range.Contains(i).Should().BeTrue();
            }

            range.Contains(10).Should().BeFalse();
            range.Contains(16).Should().BeFalse();
        }

        [Fact]
        public void WrapAroundContains()
        {
            var range = new HashRange(uint.MaxValue - 10, 10);

            range.Contains(uint.MaxValue - 10).Should().BeFalse();

            foreach (uint i in new WrappingRange(range))
            {
                range.Contains(i).Should().BeTrue();
            }

            range.Contains(11).Should().BeFalse();
        }
    }
}
