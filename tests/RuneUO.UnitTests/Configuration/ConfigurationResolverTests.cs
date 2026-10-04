// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.IO;
using RuneUO.Configuration;
using FluentAssertions;
using Xunit;

namespace RuneUO.UnitTests.Configuration
{
    public class ConfigurationResolverTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "runeuo-tests-" + Guid.NewGuid().ToString("N"));

        public ConfigurationResolverTests()
        {
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            Directory.Delete(_dir, true);
        }

        [Fact]
        public void Corrupt_File_Returns_Null_And_Is_Kept_Aside()
        {
            string file = Path.Combine(_dir, "settings.json");
            File.WriteAllText(file, "{ \"username\": ");

            var result = ConfigurationResolver.Load(file, SettingsJsonContext.RealDefault.Settings);

            result.Should().BeNull();
            File.Exists(file).Should().BeFalse();
            File.Exists(file + ".corrupt").Should().BeTrue();
        }

        [Fact]
        public void Save_Then_Load_Round_Trips_And_Leaves_No_Temp_File()
        {
            string file = Path.Combine(_dir, "settings.json");

            ConfigurationResolver.Save(new Settings { Username = "player" }, file, SettingsJsonContext.RealDefault.Settings);

            var result = ConfigurationResolver.Load(file, SettingsJsonContext.RealDefault.Settings);

            result.Username.Should().Be("player");
            File.Exists(file + ".tmp").Should().BeFalse();
        }
    }
}
