using System;
namespace Lux.Framework.ECS
{
    /// <summary>
    /// Only works with struct components
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Previous<T> : IComponent where T : IComponent
    {
        public readonly T Value;

        public Previous(T value)
        {
            Value = value;
        }
    }
}
