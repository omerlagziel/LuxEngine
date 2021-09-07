using System.Collections.Generic;

namespace Lux.Framework.ECS
{
    public class EntityGenerator
    {
        short _nextIndex;
        Stack<Entity> _destroyedEntities;

        public  EntityGenerator()
        {
            _nextIndex = 0;
            _destroyedEntities = new Stack<Entity>();
        }

        public Entity CreateEntity()
        {
            Entity entity;

            // If there are destroyed entites to recycle
            if (0 != _destroyedEntities.Count)
            {
                // Recycle entity
                entity = _destroyedEntities.Pop();
                entity.Generation++;
            }
            else
            {
                // Create new entity
                entity = new Entity
                {
                    Index = _nextIndex,
                    Generation = 0
                };

                _nextIndex++;
            }

            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            _destroyedEntities.Push(entity);
        }
    }
}
