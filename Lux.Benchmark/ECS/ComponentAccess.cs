using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Lux.Framework.ECS;

namespace Lux.Benchmark
{
    [MemoryDiagnoser]
    public class ComponentAccess
    {
        private ECS _ecs;
        private WorldHandle _noComponents;
        private WorldHandle _singleComponent;
        private WorldHandle _singleComponentSpan;
        private WorldHandle _twoComponents;
        private WorldHandle _twoComponentsSpan;

#if DEBUG
        public static int EntityCount = 10;
#else
        [Params(10, 100, 1000)]
        public static int EntityCount { get; set; }
#endif

        public struct Position : IComponent // TODO: CHANGE TO STRUCT
        {
            public float X;
            public float Y;
            public List<int> SomeList;

            public Position(int x, int y)
            {
                X = x;
                Y = y;
                SomeList = new List<int>();
            }
        }

        public struct Speed : IComponent
        {
            public float X;
            public float Y;

            public Speed(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public static void Init(Context context)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                Entity entity = context.CreateEntity();
                context.AddComponent(entity, new Position(0, 0));
                context.AddComponent(entity, new Speed(1, 2));

                if (i % 4 == 0)
                {
                    context.RemoveComponent<Position>(entity);

                    Entity entity2 = context.CreateEntity();
                    context.AddComponent(entity2, new Position(0, 0));
                    context.AddComponent(entity2, new Speed(1, 2));

                    context.AddComponent(entity, new Position(0, 0));
                    i++;
                }
            }
        }

        public class EmptyFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add(() =>
                {
                });
            }
        }

        public class SingleFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Position position) =>
                {
                    position.X += 1;
                    position.Y += 1;
                });
            }
        }

        public class SingleFeatureSpan : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Context context) =>
                {
                    var positions = context.GetAll<Position>();

                    for (int i = 0; i < positions.Length; i++)
                    {  
                        //if (positions[i] != null)
                        //{
                        positions[i].X += 1;
                        positions[i].Y += 1;
                        //}
                    }
                });
            }
        }

        public class TwoFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Position position, Speed speed) =>
                {
                    position.X += speed.X;
                    position.Y += speed.Y;
                });
            }
        }

        public class TwoFeatureSpan : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Context context) =>
                {
                    var positions = context.GetAll<Position>();
                    var speeds = context.GetAllReadonly<Speed>();

                    for (int i = 0; i < positions.Length; i++)
                    {
                        positions[i].X += speeds[i].X;
                        positions[i].Y += speeds[i].Y;
                    }
                });
            }
        }

        [IterationSetup]
        public void Setup()
        {
            _ecs = new ECS();

            _noComponents = _ecs.CreateWorld();
            _noComponents.AddFeature(new EmptyFeature());
            _noComponents.Init();

            _singleComponent = _ecs.CreateWorld();
            _singleComponent.AddFeature(new SingleFeature());
            _singleComponent.Init();

            _singleComponentSpan = _ecs.CreateWorld();
            _singleComponentSpan.AddFeature(new SingleFeatureSpan());
            _singleComponentSpan.Init();

            _twoComponents = _ecs.CreateWorld();
            _twoComponents.AddFeature(new TwoFeature());
            _twoComponents.Init();

            _twoComponentsSpan = _ecs.CreateWorld();
            _twoComponentsSpan.AddFeature(new TwoFeatureSpan());
            _twoComponentsSpan.Init();
        }

        [Benchmark]
        public void UpdateNoComponents()
        {
            for (int i = 0; i < 1000; i++)
            {
                _noComponents.Update();
            }
        }

        [Benchmark]
        public void UpdateSingleComponent()
        {
            for (int i = 0; i < 1000; i++)
            {
                _singleComponent.Update();
            }
        }

        [Benchmark]
        public void UpdateSingleComponentSpan()
        {
            for (int i = 0; i < 1000; i++)
            {
                _singleComponentSpan.Update();
            }
        }

        [Benchmark]
        public void UpdateTwoComponents()
        {
            for (int i = 0; i < 1000; i++)
            {
                _twoComponents.Update();
            }
        }

        [Benchmark]
        public void UpdateTwoComponentsSpan()
        {
            for (int i = 0; i < 1000; i++)
            {
                _twoComponentsSpan.Update();
            }
        }
    }
}
