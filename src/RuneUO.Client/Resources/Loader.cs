using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneUO.Resources
{
    public partial class Loader
    {
        [FileEmbed.FileEmbed("runeuo-logo.png")]
        public static partial ReadOnlySpan<byte> GetLogo();

        [FileEmbed.FileEmbed("game-background.png")]
        public static partial ReadOnlySpan<byte> GetBackgroundImage();
    }
}
