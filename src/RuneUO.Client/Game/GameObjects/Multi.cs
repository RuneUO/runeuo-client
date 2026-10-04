// SPDX-License-Identifier: BSD-2-Clause

using RuneUO.Game.Data;
using RuneUO.Game.Managers;
using RuneUO.Assets;

namespace RuneUO.Game.GameObjects
{
    internal sealed partial class Multi : GameObject
    {
        private ushort _originalGraphic;


        public string Name => ItemData.Name;

        public ref StaticTiles ItemData => ref Client.Game.UO.FileManager.TileData.StaticData[Graphic];
        public bool IsCustom;
        public bool IsVegetation;
        public int MultiOffsetX;
        public int MultiOffsetY;
        public int MultiOffsetZ;
        public bool IsMovable;
        public CUSTOM_HOUSE_MULTI_OBJECT_FLAGS State = 0;


        public Multi(World world) : base(world) { }

        public static Multi Create(World world, ushort graphic)
        {
            Multi m = new Multi(world); // _pool.GetOne();
            m.Graphic = m._originalGraphic = graphic;
            m.UpdateGraphicBySeason();
            m.AllowedToDraw = CanBeDrawn(world, m.Graphic);

            if (m.ItemData.Height > 5 || m.ItemData.Height == 0)
            {
                m._canBeTransparent = 1;
            }
            else if (m.ItemData.IsRoof || m.ItemData.IsSurface && m.ItemData.IsBackground || m.ItemData.IsWall)
            {
                m._canBeTransparent = 1;
            }
            else if (m.ItemData.Height == 5 && m.ItemData.IsSurface && !m.ItemData.IsBackground)
            {
                m._canBeTransparent = 1;
            }
            else
            {
                m._canBeTransparent = 0;
            }

            return m;
        }

        public override void UpdateGraphicBySeason()
        {
            Graphic = SeasonManager.GetSeasonGraphic(World.Season, _originalGraphic);
            IsVegetation = StaticFilters.IsVegetation(Graphic);
        }

        public override void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            base.Destroy();
            //_pool.ReturnOne(this);
        }
    }
}