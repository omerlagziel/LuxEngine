using NUnit.Framework;
using Lux.Framework.ECS;

namespace Lux.Tests
{
    public class EngineTests
    {
        class TestFeature : IFeature, IUpdateFeature
        {
            public void Update(Systems systems)
            {
                systems.Add((Context context) =>
                {

                });

                systems.Add(() =>
                {

                });
            }
        }

        private ECS _ecs;

        [SetUp]
        public void Setup()
        {
            _ecs = new ECS();

            WorldHandle world = _ecs.CreateWorld();
            world.AddFeature(new TestFeature());
            world.Init();
        }

        [Test]
        public void Update()
        {
            _ecs.Update();
        }
    }
}