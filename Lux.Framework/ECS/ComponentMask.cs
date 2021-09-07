using System;
namespace Lux.Framework.ECS
{
    public class ComponentMask
    {
        private readonly static int INT_SIZE = sizeof(int) * 8;
        private readonly int[] _mask;

        public ComponentMask(int maxComponents)
        {
            int size = maxComponents / INT_SIZE;
            if (maxComponents % INT_SIZE > 0)
            {
                size++;
            }

            _mask = new int[size];
        }

        public void AddComponent<T>() where T : IComponent
        {
            int index = AComponent<T>.ComponentType;
            int arrIndex = index / INT_SIZE;
            int intIndex = index % INT_SIZE;

            _mask[arrIndex] |= 1 << intIndex;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            int index = AComponent<T>.ComponentType;
            int arrIndex = index / INT_SIZE;
            int intIndex = index % INT_SIZE;

            _mask[arrIndex] &= ~(1 << intIndex);
        }

        public bool Has<T>() where T : IComponent
        {
            int index = AComponent<T>.ComponentType;
            int arrIndex = index / INT_SIZE;
            int intIndex = index % INT_SIZE;

            // Is bit on
            return 1 == ((_mask[arrIndex] >> intIndex) & 1);
        }

        /// <summary>
        /// Checks if the given mask has at least all the components
        /// of this mask.
        /// </summary>
        /// <param name="otherMask">Mask to check if contained</param>
        /// <returns>Whether the otherMask is contained in this one</returns>
        public bool Matches(ComponentMask otherMask)
        {
            if (otherMask == null)
            {
                return false;
            }

            // Masks should be the same size
            if (_mask.Length != otherMask._mask.Length)
            {
                Assert.Fail("Component masks should be the same size when compared. {0} != {1}", _mask.Length, otherMask._mask.Length);
                return false;
            }

            // Compare masks
            for (int i = 0; i < _mask.Length; i++)
            {
                if (_mask[i] != (_mask[i] & otherMask._mask[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            for (int i = 0; i < _mask.Length; i++)
            {
                _mask[i] = 0;
            }
        }

        //public bool IsEmpty()
        //{
        //    for (int i = 0; i < _mask.Length; i++)
        //    {
        //        if (_mask[i] != 0)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
