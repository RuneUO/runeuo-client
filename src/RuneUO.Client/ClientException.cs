// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace RuneUO
{
    internal class InvalidClientVersion : Exception
    {
        public InvalidClientVersion(string msg) : base(msg)
        {
        }
    }

    internal class InvalidClientDirectory : Exception
    {
        public InvalidClientDirectory(string msg) : base(msg)
        {
        }
    }
}