// SPDX-License-Identifier: BSD-2-Clause

using RuneUO.Game.Data;
using RuneUO.Game.Managers;
using Microsoft.Xna.Framework;

namespace RuneUO.Game.GameObjects
{
    internal class TextObject : BaseGameObject
    {

        public TextObject(World world) : base(world) { }

        public byte Alpha;
        public TextObject DLeft, DRight;
        public ushort Hue;
        public bool IsDestroyed;
        public bool IsTextGump;
        public bool IsTransparent;
        public GameObject Owner;

        public RenderedText RenderedText;
        public long Time, SecondTime;
        public MessageType Type;
        public int X, Y, OffsetY;


        public static TextObject Create(World world)
        {
            return new TextObject(world) { Alpha = 0xFF }; // _queue.GetOne();
        }


        public virtual void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            UnlinkD();

            RealScreenPosition = Point.Zero;
            IsDestroyed = true;
            RenderedText?.Destroy();
            RenderedText = null;
            Owner = null;
        }

        public void UnlinkD()
        {
            if (DRight != null)
            {
                DRight.DLeft = DLeft;
            }

            if (DLeft != null)
            {
                DLeft.DRight = DRight;
            }

            DRight = null;
            DLeft = null;
        }

        public void ToTopD()
        {
            TextObject obj = this;

            while (obj != null)
            {
                if (obj.DLeft == null)
                {
                    break;
                }

                obj = obj.DLeft;
            }

            TextRenderer next = (TextRenderer) obj;
            next.MoveToTop(this);
        }
    }
}