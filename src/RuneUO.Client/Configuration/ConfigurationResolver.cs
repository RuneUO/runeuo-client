// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using RuneUO.Utility.Logging;

namespace RuneUO.Configuration
{
    internal static class ConfigurationResolver
    {
        public static T Load<T>(string file, JsonTypeInfo<T> ctx) where T : class
        {
            if (!File.Exists(file))
            {
                Log.Warn(file + " not found.");

                return null;
            }

            string text;

            try
            {
                text = File.ReadAllText(file);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Log.Error($"could not read {file}: {e.Message}");

                return null;
            }

            text = Regex.Replace
            (
                text,
                @"(?<!\\)  # lookbehind: Check that previous character isn't a \
                                                \\         # match a \
                                                (?!\\)     # lookahead: Check that the following character isn't a \",
                @"\\",
                RegexOptions.IgnorePatternWhitespace
            );

            try
            {
                return JsonSerializer.Deserialize(text, ctx);
            }
            catch (JsonException e)
            {
                // Keep the broken file for the player instead of overwriting it with defaults.
                string backup = file + ".corrupt";

                Log.Error($"{file} is not valid json, moved to {backup}: {e.Message}");

                try
                {
                    File.Move(file, backup, true);
                }
                catch (Exception moveError) when (moveError is IOException || moveError is UnauthorizedAccessException)
                {
                    Log.Error($"could not move {file}: {moveError.Message}");
                }

                return null;
            }
        }

        public static void Save<T>(T obj, string file, JsonTypeInfo<T> ctx) where T : class
        {
            // this try catch is necessary when multiple client instances point to this file.
            try
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                var json = JsonSerializer.Serialize(obj, ctx);

                // Write next to the target and swap, so a crash mid-write never truncates the file.
                var temp = file + ".tmp";
                File.WriteAllText(temp, json);
                File.Move(temp, file, true);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Log.Error(e.ToString());
            }
        }
    }
}