using System;
using Lux.Framework.ECS;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace Lux.Benchmark
{
    class MainFeature : IFeature, IInitFeature
    {
        public void Init(Systems systems)
        {
            systems.Add(() =>
            {
                Console.WriteLine("Init");
            });
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(ComponentAccess),
                //typeof(ComponentCreation),
#if DEBUG
            }).RunAll(new DebugInProcessConfig());
#else
            }).RunAll();
#endif

            int x = 0;
            x += 3;
        }
    }
}
