// SPDX-License-Identifier: BSD-2-Clause

using RuneUO.Game.UI.Gumps;
using Xunit;

namespace RuneUO.UnitTests.Game.UI
{
    public class ParseMarkerTests
    {
        [Fact]
        public void Valid_Line_Is_Parsed()
        {
            var marker = WorldMapGump.ParseMarker("1496,1628,0,Britain,city,yellow,4".Split(','));

            Assert.NotNull(marker);
            Assert.Equal(1496, marker.X);
            Assert.Equal(1628, marker.Y);
            Assert.Equal(4, marker.ZoomIndex);
        }

        [Theory]
        [InlineData("1496,1628")]
        [InlineData("abc,1628,0,Britain,city,yellow")]
        [InlineData("1496,1628,0,Britain,city,yellow,zoom")]
        public void Malformed_Line_Returns_Null(string line)
        {
            Assert.Null(WorldMapGump.ParseMarker(line.Split(',')));
        }
    }
}
