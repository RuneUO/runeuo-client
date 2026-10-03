// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Runtime.InteropServices;

namespace RuneUO.Renderer.Batching
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SetIndexBufferCommand
    {
        public int type;

        public IntPtr id;
    }
}