using RuneUO.Game;
using Xunit;

namespace RuneUO.UnitTests.Game.GameObjects.Effect
{
    public class EffectManager
    {
        [Fact]
        public void Create_EffectManager_Add_Single_Effect_Then_Clear_Contents()
        {
            var world = new World();
            var em = new RuneUO.Game.Managers.EffectManager(world);

            em.CreateEffect(RuneUO.Game.Data.GraphicEffectType.FixedFrom, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, RuneUO.Game.Data.GraphicEffectBlendMode.Normal);

            Assert.NotNull(em.Items);

            em.Clear();
            world.Clear();

            Assert.Null(em.Next);
            Assert.Null(em.Previous);
            Assert.Null(em.Items);
        }

        [Fact]
        public void Create_EffectManager_Add_Multiple_Effects_Then_Clear_Contents()
        {
            var world = new World();
            var em = new RuneUO.Game.Managers.EffectManager(world);

            em.CreateEffect(RuneUO.Game.Data.GraphicEffectType.FixedFrom, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, RuneUO.Game.Data.GraphicEffectBlendMode.Normal);
            em.CreateEffect(RuneUO.Game.Data.GraphicEffectType.FixedFrom, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, RuneUO.Game.Data.GraphicEffectBlendMode.Normal);
            em.CreateEffect(RuneUO.Game.Data.GraphicEffectType.FixedFrom, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, RuneUO.Game.Data.GraphicEffectBlendMode.Normal);

            Assert.NotNull(em.Items);

            em.Clear();
            world.Clear();

            Assert.Null(em.Next);
            Assert.Null(em.Previous);
            Assert.Null(em.Items);
        }
    }
}
