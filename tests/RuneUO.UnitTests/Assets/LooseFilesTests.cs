// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.IO;
using RuneUO.Assets;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Assets
{
    public class LooseFilesTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "runeuo-tests-" + Guid.NewGuid().ToString("N"));

        public LooseFilesTests()
        {
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            Directory.Delete(_dir, true);
        }

        [Fact]
        public void Gather_Ignores_Case_Of_Folders_And_Extensions()
        {
            Directory.CreateDirectory(Path.Combine(_dir, "art", "STATICS"));
            File.WriteAllBytes(Path.Combine(_dir, "art", "STATICS", "100.ART"), new byte[1]);
            File.WriteAllBytes(Path.Combine(_dir, "art", "STATICS", "200.art"), new byte[1]);

            var found = new Dictionary<int, string>();
            LooseFiles.Gather(_dir, "Art/Statics", ".art", 0x10000, found);

            found.Keys.Should().BeEquivalentTo(new[] { 100, 200 });
        }

        [Fact]
        public void Gather_Skips_Ids_Out_Of_Range_And_Non_Numbers()
        {
            Directory.CreateDirectory(Path.Combine(_dir, "Sounds"));
            File.WriteAllBytes(Path.Combine(_dir, "Sounds", "70000.wav"), new byte[1]);
            File.WriteAllBytes(Path.Combine(_dir, "Sounds", "clang.wav"), new byte[1]);
            File.WriteAllBytes(Path.Combine(_dir, "Sounds", "5.wav"), new byte[1]);

            var found = new Dictionary<int, string>();
            LooseFiles.Gather(_dir, "Sounds", ".wav", 0xFFFF, found);

            found.Keys.Should().BeEquivalentTo(new[] { 5 });
        }

        [Fact]
        public void Missing_Folder_And_File_Do_Not_Throw()
        {
            var found = new Dictionary<int, string>();
            LooseFiles.Gather(_dir, "Gumps", ".gump", 0x10000, found);

            found.Should().BeEmpty();
            LooseFiles.Read(Path.Combine(_dir, "nope.gump")).Should().BeNull();
        }
    }
}
