//using System;
//using System.Runtime.CompilerServices;

//namespace Lux.Framework.ECS
//{
//    public readonly ref struct Components<T>
//    {
//        private readonly int[] _sparseArr;
//        private readonly Entity[] _keys;
//        private readonly T[] _components;

//        public readonly int Count;

//        internal Components(int[] sparseArr, Entity keys, T[] components, int count)
//        {
//            _sparseArr = sparseArr;
//            _keys = keys;
//            _components = components;
//            Count = count;
//        }

//        /// <summary>
//        /// Gets the component of type <typeparamref name="T"/> on the provided <see cref="Entity"/>.
//        /// </summary>
//        /// <param name="entity">The <see cref="Entity"/> for which to get the component of type <typeparamref name="T"/>.</param>
//        /// <returns>A reference to the component of type <typeparamref name="T"/>.</returns>
//        public ref T this[int index]
//        {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => ref _components[_sparseArr[index]];
//        }

//        public ref T Get(int index, out Entity entity)
//        {
//            entityIndex = _keys[_sparseArr[index]];
//            return ref _components[entityIndex];
//        }
//    }
//}
