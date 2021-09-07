using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Lux.Framework.ECS
{
    // TODO: Make sure all the component arrays are next to each other in memory
    // TODO: Reduce the seperation of Systems, if possible to only one Systems object that is filtered some other way during registration

    public class Systems
    {
        private readonly ASystem[] _systems;
        private bool _registrationDone;

        public int Count { get; private set; }

        internal Systems()
        {
            _registrationDone = false;
            _systems = new ASystem[HardCodedConfig.MAX_SYSTEMS]; // TODO: Have the user provide this
            Count = 0;
        }

        internal ASystem this[int i]
        {
            get { return _systems[i]; }
        }

        internal void Register(World world)
        {
            for (int i = 0; i < Count; i++)
            {
                _systems[i].Register(world);
            }

            _registrationDone = true;
        }

        private void Add(ASystem system)
        {
            _systems[Count] = system;
            Count++;
        }

        #region Systems Add Declerations

        public Systems Add(Action system)
        {
            if (_registrationDone)
            {
                Assert.Fail("Can't register more systems at this point.");
                return this;
            }

            Add(new System(system));
            return this;
        }

        /// <summary>
        /// Adds a system to the list
        /// </summary>
        /// <typeparam name="T1">Component that the system method operates on</typeparam>
        /// <param name="system">The system method that will be invoked</param>
        /// <returns>This instance. Enables chaining Add calls.</returns>
        public Systems Add<T1>(Action<T1> system)
            where T1 : IComponent
        {
            Add(new System<T1>(system));
            return this;
        }

        public Systems Add<T1, T2>(Action<T1, T2> system)
            where T1 : IComponent
            where T2 : IComponent
        {
            Add(new System<T1, T2>(system));
            return this;
        }

        public Systems Add<T1, T2, T3>(Action<T1, T2, T3> system)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            Add(new System<T1, T2, T3>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4>(Action<T1, T2, T3, T4> system)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            Add(new System<T1, T2, T3, T4>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> system)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            Add(new System<T1, T2, T3, T4, T5>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> system)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            Add(new System<T1, T2, T3, T4, T5, T6>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> system)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            Add(new System<T1, T2, T3, T4, T5, T6, T7>(system));
            return this;
        }

        #endregion
    }

    /// <summary>
    /// The abstract system from which all systems inherit
    /// </summary>
    public abstract class ASystem
    {
        public bool IsLocked;
        public ASystemAttribute[] SystemAttributes;
        protected HashSet<Entity> _registeredEntities;
        protected ComponentMask _componentMask;

        public ASystem()
        {
            IsLocked = false;
            SystemAttributes = null;
            _registeredEntities = new HashSet<Entity>();
            _componentMask = new ComponentMask(HardCodedConfig.MAX_GAME_COMPONENT_TYPES);
        }

        public void Lock()
        {
            Assert.IsFalse(IsLocked, "Can't take the lock of an already locked system.");
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }

        public bool HasAttribute<T>() where T : ASystemAttribute
        {
            for (int i = 0; i < SystemAttributes.Length; i++)
            {
                if (SystemAttributes[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        public void TryAddEntity(Entity entity, ComponentMask entityMask)
        {
            if (_componentMask.Matches(entityMask))
            {
                _registeredEntities.Add(entity);
            }
        }

        public void TryRemoveEntity(Entity entity, ComponentMask entityMask)
        {
            if (!_componentMask.Matches(entityMask))
            {
                _registeredEntities.Remove(entity);
            }
        }

        /// <summary>
        /// Invokes the system method
        /// </summary>
        /// <param name="world">World the system operates in</param>
        public abstract void Invoke(World world, Entity? entity);

        /// <summary>
        /// Registers all components used by the system to the world
        /// </summary>
        /// <param name="world">World to register the component to</param>
        protected abstract void RegisterComponents(World world);
        public void Register(World world)
        {
            // Get custom attributes
            SystemAttributes = (ASystemAttribute[])GetMethodInfo().GetCustomAttributes(typeof(ASystemAttribute), false);

            // Register the components the system uses
            RegisterComponents(world);
        }

        /// <summary>
        /// Registers a single component to the world
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="world">System's world</param>
        protected void RegisterComponent<T>(World world) where T : IComponent
        {
            _componentMask.AddComponent<T>();
        }

        /// <summary>
        /// Gets the method info of the underlying system method
        /// </summary>
        /// <returns>Method info of the system method</returns>
        protected abstract MethodInfo GetMethodInfo();
    }

    #region System Declerations

    public class System : ASystem
    {
        private readonly Action _system;

        public System(Action system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            _system();
        }

        protected override void RegisterComponents(World world)
        {
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    public class System<T1> : ASystem
        where T1 : IComponent
    {
        private readonly Action<T1> _system;

        public System(Action<T1> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(world.Unpack<T1>(entity.Value));
                return;
            }

            var c1 = world.GetAll<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                _system(c1[i]);
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2> : ASystem
        where T1 : IComponent
        where T2 : IComponent
    {
        private readonly Action<T1, T2> _system;

        public System(Action<T1, T2> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3> : ASystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        private readonly Action<T1, T2, T3> _system;

        public System(Action<T1, T2, T3> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value),
                    world.Unpack<T3>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e),
                    world.Unpack<T3>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4> : ASystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        private readonly Action<T1, T2, T3, T4> _system;

        public System(Action<T1, T2, T3, T4> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value),
                    world.Unpack<T3>(entity.Value),
                    world.Unpack<T4>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e),
                    world.Unpack<T3>(e),
                    world.Unpack<T4>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5> : ASystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        private readonly Action<T1, T2, T3, T4, T5> _system;

        public System(Action<T1, T2, T3, T4, T5> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value),
                    world.Unpack<T3>(entity.Value),
                    world.Unpack<T4>(entity.Value),
                    world.Unpack<T5>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e),
                    world.Unpack<T3>(e),
                    world.Unpack<T4>(e),
                    world.Unpack<T5>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5, T6> : ASystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        private readonly Action<T1, T2, T3, T4, T5, T6> _system;

        public System(Action<T1, T2, T3, T4, T5, T6> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value),
                    world.Unpack<T3>(entity.Value),
                    world.Unpack<T4>(entity.Value),
                    world.Unpack<T5>(entity.Value),
                    world.Unpack<T6>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e),
                    world.Unpack<T3>(e),
                    world.Unpack<T4>(e),
                    world.Unpack<T5>(e),
                    world.Unpack<T6>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
            RegisterComponent<T6>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5, T6, T7> : ASystem
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        private readonly Action<T1, T2, T3, T4, T5, T6, T7> _system;

        public System(Action<T1, T2, T3, T4, T5, T6, T7> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity? entity)
        {
            if (entity != null && _componentMask.Matches(world.GetEntityMask(entity.Value)))
            {
                _system(
                    world.Unpack<T1>(entity.Value),
                    world.Unpack<T2>(entity.Value),
                    world.Unpack<T3>(entity.Value),
                    world.Unpack<T4>(entity.Value),
                    world.Unpack<T5>(entity.Value),
                    world.Unpack<T6>(entity.Value),
                    world.Unpack<T7>(entity.Value)
                );

                return;
            }

            foreach (Entity e in _registeredEntities)
            {
                _system(
                    world.Unpack<T1>(e),
                    world.Unpack<T2>(e),
                    world.Unpack<T3>(e),
                    world.Unpack<T4>(e),
                    world.Unpack<T5>(e),
                    world.Unpack<T6>(e),
                    world.Unpack<T7>(e)
                );
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
            RegisterComponent<T6>(world);
            RegisterComponent<T7>(world);
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    #endregion
}
