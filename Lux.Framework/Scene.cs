using System;
using Lux.Framework.ECS;

namespace Lux.Framework
{
    public class Scene
    {
        /// <summary>
        /// Scene specific content manager for managing assets (textures, audio, etc.)
        /// Load scene specific assets here.
        /// </summary>
        public LuxContentManager Content;

        /// <summary>
        /// ECS feature that is responsible for all game logic.
        /// </summary>
        private static ECS.ECS _ecs;

        public Scene()
        {
            Content = new LuxContentManager();
            _ecs = new ECS.ECS();
        }

        public virtual void Begin()
        {
            _ecs.Initialize();
        }

        public virtual void End()
        {
        }

        public virtual void Update()
        {
            _ecs.Update();
        }

        public virtual void UpdateFixed()
        {
            _ecs.UpdateFixed();
        }

        public virtual void Draw()
        {
            _ecs.Draw();
        }

        protected WorldHandle CreateWorld()
        {
            return _ecs.CreateWorld();
        }
    }
}
