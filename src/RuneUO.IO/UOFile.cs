// SPDX-License-Identifier: BSD-2-Clause

using RuneUO.Utility.Logging;
using System;
using System.IO;

namespace RuneUO.IO
{
    public class UOFile : MMFileReader
    {
        public UOFile(string filepath) : base(File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            Entries = Array.Empty<UOFileIndex>();

            Log.Trace($"Loading file:\t\t{filepath}");
        }


        public UOFileIndex[] Entries;


        public ref UOFileIndex GetValidRefEntry(int index)
        {
            if (index < 0 || Entries == null || index >= Entries.Length)
            {
                return ref ResetInvalid();
            }

            ref UOFileIndex entry = ref Entries[index];

            if (entry.Offset < 0 || entry.Length <= 0 || entry.Offset == 0x0000_0000_FFFF_FFFF)
            {
                return ref ResetInvalid();
            }

            return ref entry;
        }

        // Callers write into the returned entry, so the shared invalid one must be clean every time.
        private static ref UOFileIndex ResetInvalid()
        {
            UOFileIndex.Invalid = default;

            return ref UOFileIndex.Invalid;
        }

        public virtual void FillEntries()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            Log.Trace($"Unloaded:\t\t{FilePath}");
        }
    }
}