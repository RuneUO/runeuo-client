using ClassicUO.Assets;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Renderer.Gumps
{
    public sealed class Gump
    {
        private readonly TextureAtlas _atlas;
        private readonly SpriteInfo[] _spriteInfos;
        private readonly bool[] _missing;
        private readonly PixelPicker _picker = new PixelPicker();
        private readonly GumpsLoader _gumpsLoader;

        public Gump(GumpsLoader gumpsLoader, GraphicsDevice device)
        {
            _gumpsLoader = gumpsLoader;
            _atlas = new TextureAtlas(device, 4096, 4096, SurfaceFormat.Color);
            // Sized for every id a shard can add as a loose file, not only the archive's.
            _spriteInfos = new SpriteInfo[System.Math.Max(gumpsLoader.File.Entries.Length, GumpsLoader.MAX_GUMP_DATA_INDEX_COUNT)];
            _missing = new bool[_spriteInfos.Length];
        }

        public void Reload()
        {
            // Only ids that were or now are loose files can change. Clearing everything would
            // upload every gump again into new atlas space that is never freed.
            var stale = new HashSet<int>(_gumpsLoader.OurIds);
            _gumpsLoader.LoadOurs();
            stale.UnionWith(_gumpsLoader.OurIds);

            foreach (int id in stale)
            {
                if (id < _spriteInfos.Length)
                {
                    _spriteInfos[id] = default;
                    _missing[id] = false;
                    _picker.Remove((ulong)id);
                }
            }
        }

        public ref readonly SpriteInfo GetGump(uint idx)
        {
            if (idx >= _spriteInfos.Length)
                return ref SpriteInfo.Empty;

            ref var spriteInfo = ref _spriteInfos[idx];

            if (spriteInfo.Texture == null && !_missing[idx])
            {
                var gumpInfo = _gumpsLoader.GetGump(idx);

                // Remember a missing id, or it is looked up again on every frame it is drawn.
                _missing[idx] = gumpInfo.Pixels.IsEmpty;

                if (!gumpInfo.Pixels.IsEmpty)
                {
                    spriteInfo.Texture = _atlas.AddSprite(
                        gumpInfo.Pixels,
                        gumpInfo.Width,
                        gumpInfo.Height,
                        out spriteInfo.UV
                    );

                    _picker.Set(idx, gumpInfo.Width, gumpInfo.Height, gumpInfo.Pixels);
                }
            }

            return ref spriteInfo;
        }

        public bool PixelCheck(uint idx, int x, int y) => _picker.Get(idx, x, y);
    }
}
