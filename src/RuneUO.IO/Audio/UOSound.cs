// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace RuneUO.IO.Audio
{
    public class UOSound : Sound
    {
        private readonly byte[] _waveBuffer;

        public UOSound(string name, int index, byte[] buffer) : base(name, index)
        {
            _waveBuffer = buffer;
            Delay = (uint) ((buffer.Length - 32) / 88.2f);
        }

        public bool CalculateByDistance { get; set; }
        public int X, Y;

        protected override void OnBufferNeeded(object sender, EventArgs e)
        {
        }

        protected override ArraySegment<byte> GetBuffer()
        {
            return _waveBuffer;
        }
    }
}
