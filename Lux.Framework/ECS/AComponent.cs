using System;
namespace Lux.Framework.ECS
{
    public interface IComponent
    {
    }

    /// <summary>
    /// Limits a component type to only exist on the global Singleton Entity.
    /// </summary>
    public interface ISingleton : IUnique
    {
    }

    /// <summary>
    /// Limits a component type to exist only on one entity at a time.
    /// </summary>
    public interface IUnique
    {
    }

    /// <summary>
    /// You shouldn't inherit from this.   
    /// </summary>
    [Serializable]
    public abstract class AInternalComponent
    {
        protected static int ComponentTypesCount = 0;
    }

    [Serializable]
    public sealed class AComponent<T> : AInternalComponent where T : IComponent
    {
        private static int _componentType = -1;
        public static int ComponentType
        {
            get
            {
                if (_componentType == -1)
                {
                    // If there are too many component types
                    if (ComponentTypesCount >= HardCodedConfig.MAX_GAME_COMPONENT_TYPES)
                    {
                        Assert.Fail("Too many component types are registered!"); // TOOD: Log and maybe throw
                        return _componentType;
                    }

                    _componentType = ComponentTypesCount;
                    ComponentTypesCount++;
                }

                return _componentType;
            }
        }

        public static bool IsComponentTypeSet()
        {
            return _componentType != -1;
        }
    }
}
