// SPDX-License-Identifier: BSD-2-Clause

using System.IO;
using RuneUO.IO;
using Xunit;

namespace RuneUO.UnitTests.IO
{
    public class MMFileReaderTests
    {
        [Fact]
        public void Empty_File_Reads_Nothing()
        {
            string path = Path.GetTempFileName();

            try
            {
                using var stream = File.OpenRead(path);
                using var reader = new MMFileReader(stream);

                Assert.NotNull(reader.Reader);
                Assert.Equal(0, reader.Read(new byte[4]));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
