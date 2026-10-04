// SPDX-License-Identifier: BSD-2-Clause

using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace RuneUO.Renderer.Batching
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexDeclarationCommand
    {
        public int Offset;
        public VertexElementFormat Format;
        public VertexElementUsage Usage;
        public int UsageIndex;
    }
}