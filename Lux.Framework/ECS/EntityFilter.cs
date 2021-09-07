using System;
namespace Lux.Framework.ECS
{
    public interface ISystemFilter
    {
        /// <summary>
        /// Applies a filter that determines if the system should run or not
        /// </summary>
        /// <param name="system">The system to filter</param>
        /// <returns><c>true</c> if the system should run, <c>false</c> otherwise.</returns>
        bool Filter(ASystem system);
    }
}
