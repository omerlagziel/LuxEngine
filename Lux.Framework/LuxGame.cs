using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lux.Framework
{
    public class LuxGame : Game
    {
        /// <summary>
        /// Game's title
        /// </summary>
        public static string Title { get; private set; }

        /// <summary>
        /// Provides global access to the game's instance
        /// </summary>
        public static LuxGame Instance;

        /// <summary>
        /// Content manager for managing global assets (textures, audio, etc.)
        /// </summary>
        public new static LuxContentManager Content;

        /// <summary>
        /// Provides easy access to the GraphicsDevice
        /// </summary>
        public new static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Base directory for all of the game's assets
        /// </summary>
        public static string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Instance.Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Instance.Content.RootDirectory); }
#elif XBOXONE
            get { return Instance.Content.RootDirectory; }
#else
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    ((Game)Instance).Content.RootDirectory);
            }
#endif
        }

        /// <summary>
        /// Current scene
        /// </summary>
        public static Scene Scene
        {
            get => _scene;
            set
            {
                Assert.IsNotNull(value, "Scene cannot be set to null."); 
                _nextScene = value;
            }
        }
        private static Scene _scene;

        /// <summary>
        /// Next scene to load.
        /// When this scene is set, we load it and set it to null.
        /// </summary>
        private static Scene _nextScene;


        public LuxGame(int width, int height, string windowTitle, bool fullscreen, string contentDirectory = "Content")
        {
            Instance = this;
            Window.Title = Title = windowTitle;

            Screen.Initialize(new GraphicsDeviceManager(this), width, height, fullscreen);

            base.Content.RootDirectory = contentDirectory;
            Content = new LuxContentManager(Services, base.Content.RootDirectory);

            IsMouseVisible = false;
            IsFixedTimeStep = false;

            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }
        

        //protected override void OnActivated(object sender, EventArgs args)
        //{
        //    base.OnActivated(sender, args);
        //}

        //protected override void OnDeactivated(object sender, EventArgs args)
        //{
        //    base.OnDeactivated(sender, args);
        //}

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize(); // calls LoadContent

            // Set the static graphics device
            GraphicsDevice = base.GraphicsDevice;
        }


        // TODO: We got rid of loadcontent so make sure it's working on device reset in monogame.
        // TODO: Handle buffer overflow with recycled entities' generation int

        /// <summary>
        /// Called each frame to update the game.
        /// We implement our own 
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime.TotalGameTime.TotalSeconds);

            // If should change scenes
            if (_nextScene != null)
            {
                ChangeToNextScene();
            }

            Scene.Update();

            // If accumulated enough time to run a tick, run ticks
            while (Time.Accumulator >= Time.Timestep)
            {
                Time.Tick();
                Scene.UpdateFixed();
            }

#if FNA
            // MonoGame only updates old-school XNA Components in Update which we dont care about. FNA's core FrameworkDispatcher needs
            // base.Update called though so we do so here.
            FrameworkDispatcher.Update();
#endif
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            Time.Draw(gameTime.ElapsedGameTime.TotalSeconds);
            Scene.Draw();
        }

        private void ChangeToNextScene()
        {
            _scene?.End();

            // Change nextScene to be the current scene
            _scene = _nextScene;
            _nextScene = null;

            // Update time
            Time.SceneChanged();
            _scene.Begin();

            // A good opportunity to garbage collect
            GC.Collect();
        }
    }
}
