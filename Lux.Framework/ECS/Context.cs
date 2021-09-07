using System;

namespace Lux.Framework.ECS
{
    /// <summary>
    /// The way systems interact with the world and create entities.
    /// </summary>
    /// TODO: Support Context for creating entities from inside systems
    /// TODO: Try extending Context with functions like CreateRandomMob(params MobTypes[] types)!
    public class Context : IComponent
    {
        public Entity Entity;
        private readonly World _world;

        internal Context(World world, Entity entity)
        {
            _world = world;
            Entity = entity;
        }

        public Entity CreateEntity()
        {
            return _world.CreateEntity();
        }

        public void DestroyEntity()
        {
            _world.DestroyEntity(Entity);
        }

        public void DestroyEntity(Entity entity)
        {
            _world.DestroyEntity(entity);
        }

        public void AddComponent<T>(T component, Entity entity) where T : IComponent
        {
            _world.AddComponent(entity, component);
        }

        public void AddComponentToCurrentEntity<T>(T component) where T : IComponent
        {
            AddComponent(component, Entity);
        }

        public void SetComponent<T>(T component, Entity entity) where T : IComponent
        {
            _world.SetComponent(entity, component);
        }

        public void SetComponentOfCurrentEntity<T>(T component) where T : IComponent
        {
            SetComponent(component, Entity);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            _world.RemoveComponent<T>(entity);
        }

        public void RemoveComponentFromCurrentEntity<T>() where T : IComponent
        {
            RemoveComponent<T>(Entity);
        }

        public void AddSingleton<T>(T component) where T : IComponent
        {
            _world.AddSingletonComponent(component);
        }

        public void RemoveSingleton<T>() where T : IComponent
        {
            _world.RemoveSingletonComponent<T>();
        }

        public bool Unpack<T>(out T component, Entity entity) where T : IComponent
        {
            return _world.Unpack(entity, out component);
        }

        public bool Unpack<T>(out T component) where T : IComponent
        {
            return Unpack(out component, Entity);
        }

        public bool UnpackSingleton<T>(out T component) where T : IComponent
        {
            return _world.UnpackSingleton(out component);
        }

        public bool UnpackUnique<T>(out T component) where T : IComponent
        {
            ReadOnlySpan<T> span = _world.GetAllReadonly<T>();

            switch (span.Length)
            {
                case 0:
                    component = default;
                    return false;
                case 1:
                    break;
                default:
                    Assert.Fail("Unique component type can't have more then 1 component. Did you mean to use context.Unpack()?");
                    break;
            }

            component = span[0];
            return true;
        }

        public Span<T> GetAll<T>() where T : IComponent
        {
            return _world.GetAll<T>();
        }

        public ReadOnlySpan<T> GetAllReadonly<T>() where T : IComponent
        {
            return _world.GetAllReadonly<T>();
        }

        public ReadOnlySpan<T> GetAllReadonly<T>(out ReadOnlySpan<Entity> entities) where T : IComponent
        {
            return _world.GetAllReadonly<T>(out entities);
        }
    }
}
