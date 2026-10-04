// SPDX-License-Identifier: BSD-2-Clause

using RuneUO.IO;
using RuneUO.Utility;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RuneUO.Assets
{
    public sealed class HuesLoader : UOFileLoader
    {
        public HuesLoader(UOFileManager fileManager) : base(fileManager)
        {
        }

        public HuesGroup[] HuesRange { get; private set; }

        public int HuesCount { get; private set; }

        public FloatHues[] Palette { get; private set; }

        public ushort[] RadarCol { get; private set; }

        public override unsafe void Load()
        {
            var path = FileManager.GetUOFilePath("hues.mul");

            FileSystemHelper.EnsureFileExists(path);

            using var file = new UOFileMul(path);
            int groupSize = Unsafe.SizeOf<HuesGroup>();
            int entrycount = (int) file.Length / groupSize;
            HuesCount = entrycount * 8;
            HuesRange = new HuesGroup[entrycount];

            for (int i = 0; i < entrycount; i++)
            {
                HuesRange[i] = file.Read<HuesGroup>();
            }

            path = FileManager.GetUOFilePath("radarcol.mul");

            FileSystemHelper.EnsureFileExists(path);

            using var radarcol = new UOFileMul(path);
            RadarCol = new ushort[radarcol.Length / sizeof(ushort)];
            radarcol.Read(MemoryMarshal.AsBytes<ushort>(RadarCol.AsSpan()));
        }

        public void CreateShaderColors(uint[] buffer)
        {
            int len = HuesRange.Length;

            int idx = 0;

            for (int r = 0; r < len; r++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        buffer[idx++] = HuesHelper.Color16To32(HuesRange[r].Entries[y].ColorTable[x]) | 0xFF_00_00_00;

                        if (idx >= buffer.Length)
                        {
                            return;
                        }
                    }
                }
            }
        }

        //        return Palette[color - 1].Palette;
        //    }

        //    return _empty;
        //}

        public ushort GetColor16(ushort c, ushort color)
        {
            if (color != 0 && color < HuesCount)
            {
                color -= 1;
                int g = color >> 3;
                int e = color % 8;

                return HuesRange[g].Entries[e].ColorTable[(c >> 10) & 0x1F];
            }

            return c;
        }

        public uint GetPolygoneColor(ushort c, ushort color)
        {
            if (color != 0 && color < HuesCount)
            {
                color -= 1;
                int g = color >> 3;
                int e = color % 8;

                return HuesHelper.Color16To32(HuesRange[g].Entries[e].ColorTable[c]);
            }

            return 0xFF010101;
        }

        public uint GetColor(ushort c, ushort color)
        {
            if (color != 0 && color < HuesCount)
            {
                color -= 1;
                int g = color >> 3;
                int e = color % 8;

                return HuesHelper.Color16To32(HuesRange[g].Entries[e].ColorTable[(c >> 10) & 0x1F]);
            }

            return color != 0 ? HuesHelper.Color16To32(color) : HuesHelper.Color16To32(c);
        }

        public uint GetPartialHueColor(ushort c, ushort color)
        {
            if (color != 0 && color < HuesCount)
            {
                color -= 1;
                int g = color >> 3;
                int e = color % 8;
                uint cl = HuesHelper.Color16To32(c);

                byte R = (byte) (cl & 0xFF);
                byte G = (byte) ((cl >> 8) & 0xFF);
                byte B = (byte) ((cl >> 16) & 0xFF);

                if (R == G && R == B)
                {
                    cl = HuesHelper.Color16To32(HuesRange[g].Entries[e].ColorTable[(c >> 10) & 0x1F]);
                }

                return cl;
            }

            return HuesHelper.Color16To32(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetRadarColorData(int c)
        {
            if (c >= 0 && c < RadarCol.Length)
            {
                return RadarCol[c];
            }

            return 0;
        }
    }


    [InlineArray(32)]
    public struct ColorTableArray
    {
        private ushort _a0;
    }

    [InlineArray(8)]
    public struct HuesBlockArray
    {
        private HuesBlock _a0;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HuesBlock
    {
        public ColorTableArray ColorTable;
        public ushort TableStart;
        public ushort TableEnd;
        public unsafe fixed byte Name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HuesGroup
    {
        public uint Header;
        public HuesBlockArray Entries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VerdataHuesGroup
    {
        public readonly uint Header;
        public HuesBlockArray Entries;
    }

    public struct FloatHues
    {
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public float[] Palette;
    }
}