// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Utility.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClassicUO.Assets
{
    /// <summary>
    /// Shard assets kept as loose files named by id (Sounds/12.wav, Gumps/40000.gump, ...).
    ///
    /// Folder and file names are matched ignoring case, so a shard packed on Windows works the
    /// same on Linux, and an unreadable folder or file is logged instead of thrown.
    /// </summary>
    internal static class LooseFiles
    {
        private static readonly EnumerationOptions _ignoreCase = new EnumerationOptions
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            IgnoreInaccessible = true
        };

        public static void Gather(string basePath, string relativeFolder, string extension, int limit, Dictionary<int, string> into)
        {
            string folder = FindFolder(basePath, relativeFolder);

            if (folder == null)
            {
                return;
            }

            try
            {
                foreach (string path in Directory.EnumerateFiles(folder, "*" + extension, _ignoreCase))
                {
                    if (int.TryParse(Path.GetFileNameWithoutExtension(path), out int id) && id >= 0 && id < limit)
                    {
                        into[id] = path;
                    }
                }
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Log.Warn($"could not list {folder}: {e.Message}");
            }
        }

        public static byte[] Read(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Log.Warn($"could not read {path}: {e.Message}");

                return null;
            }
        }

        private static string FindFolder(string basePath, string relativeFolder)
        {
            string current = basePath;

            foreach (string part in relativeFolder.Split('/'))
            {
                string exact = Path.Combine(current, part);

                if (Directory.Exists(exact))
                {
                    current = exact;

                    continue;
                }

                string match = null;

                try
                {
                    foreach (string dir in Directory.EnumerateDirectories(current, part, _ignoreCase))
                    {
                        match = dir;

                        break;
                    }
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                    return null;
                }

                if (match == null)
                {
                    return null;
                }

                current = match;
            }

            return current;
        }
    }
}
