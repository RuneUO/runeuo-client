using RuneUO.Game;
using Xunit;

namespace RuneUO.UnitTests.Game.GameObjects.Mobile
{
    public class Create
    {
        [Fact]
        public void Create_Returns_Mobile_Instance()
        {
            var world = new World();
            Assert.IsType<RuneUO.Game.GameObjects.Mobile>( RuneUO.Game.GameObjects.Mobile.Create(world, 0));
            world.Clear();
        }
    }
}
