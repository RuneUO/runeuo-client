using RuneUO.Game;
using System;
using Xunit;

namespace RuneUO.UnitTests.Game.GameObjects.Effect
{
    public class Create
    {
        [Theory]
        [InlineData((int)RuneUO.Game.Data.GraphicEffectType.FixedXYZ, typeof(RuneUO.Game.GameObjects.FixedEffect))]
        [InlineData((int)RuneUO.Game.Data.GraphicEffectType.FixedFrom, typeof(RuneUO.Game.GameObjects.FixedEffect))]
        [InlineData((int)RuneUO.Game.Data.GraphicEffectType.DragEffect, typeof(RuneUO.Game.GameObjects.DragEffect))]
        [InlineData((int)RuneUO.Game.Data.GraphicEffectType.Moving, typeof(RuneUO.Game.GameObjects.MovingEffect))]
        [InlineData((int)RuneUO.Game.Data.GraphicEffectType.Lightning, typeof(RuneUO.Game.GameObjects.LightningEffect))]
        public void Create_Returns_Effect_Instance(int graphicEffectType, Type type)
        {
            var world = new World();
            var em = new RuneUO.Game.Managers.EffectManager(world);

            em.CreateEffect((RuneUO.Game.Data.GraphicEffectType) graphicEffectType, 0, 0, 1, 0,0, 0 , 0,0 ,0,0 ,0, 0, false, false, false, RuneUO.Game.Data.GraphicEffectBlendMode.Normal);
            
            Assert.IsType(type, em.Items);

            em.Clear();
            world.Clear();
        }
    }
}
