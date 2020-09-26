using Audrey;
using System;
using System.Diagnostics;

namespace Audrey.Benchmark
{
    class Program
    {
        class ComponentA : IComponent
        {
        }
        class ComponentB : IComponent
        {
        }
        class ComponentC : IComponent
        {
        }
        class ComponentD : IComponent
        {
        }
        class ComponentE : IComponent
        {
        }
        class ComponentF : IComponent
        {
        }
        class ComponentG : IComponent
        {
        }

        static void Main(string[] args)
        {
            Engine engine = new Engine();

            Family family1 = Family.All(typeof(ComponentA), typeof(ComponentB)).Get();
            engine.GetEntitiesFor(family1);
            Family family2 = Family.All(typeof(ComponentA), typeof(ComponentC)).Get();
            engine.GetEntitiesFor(family2);
            Family family3 = Family.All(typeof(ComponentA), typeof(ComponentE)).Get();
            engine.GetEntitiesFor(family3);
            Family family4 = Family.All(typeof(ComponentA), typeof(ComponentF)).Get();
            engine.GetEntitiesFor(family4);
            Family family5 = Family.All(typeof(ComponentF)).Exclude(typeof(ComponentD)).Get();
            engine.GetEntitiesFor(family5);
            Family family6 = Family.All(typeof(ComponentF)).Exclude(typeof(ComponentE)).Get();
            engine.GetEntitiesFor(family6);
            Family family7 = Family.All(typeof(ComponentD)).Exclude(typeof(ComponentC)).Get();
            engine.GetEntitiesFor(family7);
            Family family8 = Family.All(typeof(ComponentD)).Exclude(typeof(ComponentA)).Get();
            engine.GetEntitiesFor(family8);

            Stopwatch stopwatch = new Stopwatch();

            // Add entities
            stopwatch.Start();
            int count = 10000;
            for(int i = 0; i < count; i++)
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new ComponentA());
                entity.AddComponent(new ComponentF());
                entity.AddComponent(new ComponentD());
                entity.AddComponent(new ComponentG());
                entity.AddComponent(new ComponentE());
                entity.RemoveComponent<ComponentF>();
            }
            stopwatch.Stop();

            Console.WriteLine($"Added and removed components from {count} entities, which took:");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
