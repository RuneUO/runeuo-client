// SPDX-License-Identifier: BSD-2-Clause

using System.Linq;
using RuneUO.Assets;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Assets
{
    public class ArtRunsTests
    {
        private static byte[] ToBytes(params ushort[] words)
        {
            return words.SelectMany(w => new[] { (byte)w, (byte)(w >> 8) }).ToArray();
        }

        [Fact]
        public void Valid_Row_Is_Decoded()
        {
            var buf = ToBytes(0, 0, 2, 0x7FFF, 0x7FFF, 0, 0);

            var pixels = ArtLoader.Runs(buf, 2, 1);

            pixels.Should().NotBeNull();
            pixels.Should().OnlyContain(p => p != 0);
        }

        [Fact]
        public void Row_Offset_Past_End_Returns_Null()
        {
            var buf = ToBytes(500, 0, 0);

            ArtLoader.Runs(buf, 2, 1).Should().BeNull();
        }

        [Fact]
        public void Missing_Row_Terminator_Returns_Null()
        {
            var buf = ToBytes(0, 0, 1, 0x7FFF);

            ArtLoader.Runs(buf, 2, 1).Should().BeNull();
        }

        [Fact]
        public void Run_Past_Picture_Returns_Null()
        {
            var buf = ToBytes(0, 1, 2, 0x7FFF, 0x7FFF, 0, 0);

            ArtLoader.Runs(buf, 2, 1).Should().BeNull();
        }

        [Fact]
        public void Row_Table_Larger_Than_Buffer_Returns_Null()
        {
            ArtLoader.Runs(ToBytes(0), 2, 4).Should().BeNull();
        }
    }
}
