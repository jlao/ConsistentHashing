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
    }
}
