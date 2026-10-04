using System;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Game.SerialHelper
{
    public class Parse
    {
        [Theory]
        [InlineData("1")]
        [InlineData("0x1F")]
        [InlineData("-23")]
        public void Parse_Should_Return_Legal_Number(string input)
        {
            RuneUO.Game.SerialHelper.Parse(input)
                .Should().BePositive();
        }

        [Theory]
        [InlineData("0XF")]
        [InlineData("XF")]
        [InlineData("1F")]
        public void Parse_Should_Not_Return_Legal_Number(string input)
        {
            Action act = () => RuneUO.Game.SerialHelper.Parse(input);

            act.Should().Throw<FormatException>();
        }
    }
}