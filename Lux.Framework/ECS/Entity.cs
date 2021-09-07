using System;
using System.Runtime.InteropServices;

namespace Lux.Framework.ECS
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Entity : ISparseSetKey, IEquatable<Entity>, IComparable<Entity>
    {
        [FieldOffset(0)] public Int32 Id;

        [NonSerialized] [FieldOffset(0)] private Int16 _index;
        public Int16 Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        [NonSerialized] [FieldOffset(2)] public Int16 Generation;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Entity otherEntity))
            {
                return false;
            }

            return Id.Equals(otherEntity.Id);
        }

        bool IEquatable<Entity>.Equals(Entity other)
        {
            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(Entity other)
        {
            if (other == null)
            {
                return 1;
            }

            int generationCompare = Generation.CompareTo(other.Generation);

            // If the generations are different, return that comparison
            if (0 != generationCompare)
            {
                return generationCompare;
            }

            // If the generations are the same, compare indexes
            return Index.CompareTo(other.Index);
        }
    }
}
