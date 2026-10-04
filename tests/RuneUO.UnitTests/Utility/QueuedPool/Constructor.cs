using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Utility.QueuedPool
{
    public class Constructor
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(32)]
        public void Constructor_MaxSize_Should_Populate_Items(int input)
        {
            var objectPool = new RuneUO.Utility.QueuedPool<DummyItem>(input);
            objectPool.MaxSize.Should().Be(input);
        }
    }
}