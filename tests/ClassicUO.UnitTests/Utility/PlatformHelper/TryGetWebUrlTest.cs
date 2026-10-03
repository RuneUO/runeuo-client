// SPDX-License-Identifier: BSD-2-Clause

using Xunit;

namespace ClassicUO.UnitTests.Utility.PlatformHelper
{
    public class TryGetWebUrlTest
    {
        [Theory]
        [InlineData("https://example.com/page", "https://example.com/page")]
        [InlineData("http://example.com", "http://example.com/")]
        [InlineData("www.example.com", "http://www.example.com/")]
        public void Accepts_Web_Urls(string url, string expected)
        {
            Assert.True(ClassicUO.Utility.Platforms.PlatformHelper.TryGetWebUrl(url, out string result));
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("file:///C:/Windows/System32/calc.exe")]
        [InlineData("ftp://example.com")]
        [InlineData("javascript://alert(1)")]
        [InlineData("C:\\Windows\\System32\\calc.exe")]
        [InlineData("\\\\server\\share\\app.exe")]
        public void Rejects_Non_Web_Urls(string url)
        {
            Assert.False(ClassicUO.Utility.Platforms.PlatformHelper.TryGetWebUrl(url, out _));
        }
    }
}
