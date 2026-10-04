// SPDX-License-Identifier: BSD-2-Clause

using Xunit;

namespace RuneUO.UnitTests.Utility.StringHelper
{
    public class Cp1252ToStringTests
    {
        [Fact]
        public void Converts_Ascii_And_Cp1252_Specific_Characters()
        {
            byte[] data = { (byte)'U', (byte)'O', 0x80, 0x99, 0xE9 };

            Assert.Equal("UO€™é", RuneUO.Utility.StringHelper.Cp1252ToString(data));
        }

        [Fact]
        public void Empty_Input_Returns_Empty_String()
        {
            Assert.Equal(string.Empty, RuneUO.Utility.StringHelper.Cp1252ToString(System.ReadOnlySpan<byte>.Empty));
        }
    }
}
