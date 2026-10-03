using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Utility.FileSystemHelper
{
    public class EnsureFileExists
    {
        [Fact]
        public void EnsureFileExists_For_InvalidPath_Should_ThrowException()
        {
            Action act = () => RuneUO.Utility.FileSystemHelper.EnsureFileExists("abc\\invalid_file\\name.extension");

            act.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void EnsureFileExists_ValidPath_Should_Not_ThrowException()
        {
            var validFileName = Path.GetTempFileName();

            Action act = () => RuneUO.Utility.FileSystemHelper.EnsureFileExists(validFileName);

            act.Should().NotThrow<FileNotFoundException>();

            File.Delete(validFileName);
        }
    }
}