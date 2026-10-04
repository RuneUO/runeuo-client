using System.IO;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Utility.FileSystemHelper
{
    public class CreateFolderIfNotExists
    {
        [Fact]
        public void When_ValidPath_Provided_Directories_Will_BeCreated_And_Valid_Path_Returned()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), nameof(CreateFolderIfNotExists));

            var createdPath = RuneUO.Utility.FileSystemHelper.CreateFolderIfNotExists(tempPath, "Part1", "Part2");

            createdPath.Should().BeEquivalentTo(Path.Combine(tempPath, "Part1", "Part2"));

            Directory.Delete(tempPath, true);
        }

        [Theory]
        [InlineData("..")]
        [InlineData(".")]
        [InlineData("../../etc")]
        [InlineData("..\\..\\Windows")]
        public void Parts_From_The_Server_Stay_Inside_The_Base_Folder(string part)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), nameof(CreateFolderIfNotExists) + "_escape");

            var createdPath = RuneUO.Utility.FileSystemHelper.CreateFolderIfNotExists(tempPath, part, "Character");

            Path.GetFullPath(createdPath).Should().StartWith(Path.GetFullPath(tempPath) + Path.DirectorySeparatorChar);

            Directory.Delete(tempPath, true);
        }
    }
}