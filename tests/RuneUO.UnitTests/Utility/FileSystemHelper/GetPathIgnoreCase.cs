using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Utility.FileSystemHelper
{
    public class GetPathIgnoreCase : IDisposable
    {
        private readonly string _root;

        public GetPathIgnoreCase()
        {
            _root = Path.Combine(Path.GetTempPath(), "runeuo-ignorecase-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(_root, "music", "digital"));
            File.WriteAllText(Path.Combine(_root, "Tiledata.mul"), "");
            File.WriteAllText(Path.Combine(_root, "music", "digital", "Config.txt"), "");
        }

        public void Dispose()
        {
            Directory.Delete(_root, true);
        }

        [Fact]
        public void Finds_A_File_Named_With_Other_Casing()
        {
            string path = RuneUO.Utility.FileSystemHelper.GetPathIgnoreCase(_root, "tiledata.mul");

            File.Exists(path).Should().BeTrue();
        }

        [Fact]
        public void Finds_Folders_Named_With_Other_Casing()
        {
            RuneUO.Utility.FileSystemHelper.GetPathIgnoreCase(_root, "Music/Digital").Should().Match(p => Directory.Exists(p));
            File.Exists(RuneUO.Utility.FileSystemHelper.GetPathIgnoreCase(_root, "Music/Digital/config.txt")).Should().BeTrue();
        }

        [Fact]
        public void Returns_The_Path_Unchanged_When_Nothing_Matches()
        {
            RuneUO.Utility.FileSystemHelper.GetPathIgnoreCase(_root, "hues.mul").Should().Be(Path.Combine(_root, "hues.mul"));
        }
    }
}
