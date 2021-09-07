using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lux.Framework
{
    /// <summary>
    /// Represents the game's screen and provides resolution information.
    /// </summary>
    public static class Screen
    {
        private static GraphicsDeviceManager _graphicsManager;

		/// <summary>
		/// Is currently resizing the window
		/// </summary>
		private static bool resizing;

		/// <summary>
		/// The amount of padding to apply on the view
		/// </summary>
		public static int ViewPadding
		{
			get { return _viewPadding; }
			set
			{
				_viewPadding = value;
				UpdateView();
			}
		}
		private static int _viewPadding = 0;

		/// <summary>
		/// Window's viewport
		/// </summary>
		public static Viewport Viewport { get; private set; }

		/// <summary>
		/// Screen's transformation matrix
		/// </summary>
		public static Matrix ScreenMatrix = Matrix.Identity;

		/// <summary>
		/// Width of the GraphicsDevice's back buffer
		/// </summary>
		public static int Width
		{
			get => _graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
			set => _graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth = value;
		}

		/// <summary>
		/// Height of the GraphicsDevice's back buffer
		/// </summary>
		public static int Height
		{
			get => _graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight;
			set => _graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight = value;
		}

		/// <summary>
		/// The center of the screen/backbuffer. If you are rendering to a smaller RenderTarget
		/// you will need to scale this value appropriately.
		/// </summary>
		public static Vector2 Center => new Vector2(Width / 2, Height / 2);

        /// <summary>
        /// Preferred width of the backbuffer
        /// </summary>
		public static int PreferredBackBufferWidth
		{
			get => _graphicsManager.PreferredBackBufferWidth;
			set => _graphicsManager.PreferredBackBufferWidth = value;
		}

		/// <summary>
		/// Preferred height of the backbuffer
		/// </summary>
		public static int PreferredBackBufferHeight
		{
			get => _graphicsManager.PreferredBackBufferHeight;
			set => _graphicsManager.PreferredBackBufferHeight = value;
		}

        /// <summary>
        /// Width of the monitor the game is running on
        /// </summary>
		public static int MonitorWidth => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

		/// <summary>
		/// Height of the monitor the game is running on
		/// </summary>
		public static int MonitorHeight => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


		[Conditional(HardCodedConfig.NCONSOLE)]
		public static void SetWindowed(int width, int height)
		{
			if (width > 0 && height > 0)
			{
				resizing = true;
				_graphicsManager.PreferredBackBufferWidth = width;
				_graphicsManager.PreferredBackBufferHeight = height;
				_graphicsManager.IsFullScreen = false;
				_graphicsManager.ApplyChanges();
				resizing = false;
			}
		}

		[Conditional(HardCodedConfig.NCONSOLE)]
		public static void SetFullscreen()
		{
			resizing = true;
			_graphicsManager.PreferredBackBufferWidth = MonitorWidth;
			_graphicsManager.PreferredBackBufferHeight = MonitorHeight;
			_graphicsManager.IsFullScreen = true;
			_graphicsManager.ApplyChanges();
			resizing = false;
		}


		internal static void Initialize(GraphicsDeviceManager graphicsManager, int width, int height, bool fullscreen)
		{
			_graphicsManager = graphicsManager;

#if PS4 || XBOXONE
            _graphicsManager.PreferredBackBufferWidth = 1920;
            _graphicsManager.PreferredBackBufferHeight = 1080;
			_graphicsManager.IsFullScreen = fullscreen;
#elif NSWITCH
            _graphicsManager.PreferredBackBufferWidth = 1280;
            _graphicsManager.PreferredBackBufferHeight = 720;
			_graphicsManager.IsFullScreen = fullscreen;
#else
			_graphicsManager.PreferredBackBufferWidth = fullscreen ? MonitorWidth : width;
			_graphicsManager.PreferredBackBufferHeight = fullscreen ? MonitorHeight : height;
			_graphicsManager.IsFullScreen = fullscreen;

			LuxGame.Instance.Window.AllowUserResizing = true;
			LuxGame.Instance.Window.ClientSizeChanged += OnClientSizeChanged;
#endif

			_graphicsManager.SynchronizeWithVerticalRetrace = true;
			_graphicsManager.PreferMultiSampling = false;
			_graphicsManager.GraphicsProfile = GraphicsProfile.HiDef;
			_graphicsManager.PreferredBackBufferFormat = SurfaceFormat.Color;
			_graphicsManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			_graphicsManager.DeviceReset += OnGraphicsReset;
			_graphicsManager.DeviceCreated += OnGraphicsCreate;

			_graphicsManager.ApplyChanges();
		}

		private static void OnGraphicsReset(object sender, EventArgs e)
		{
			UpdateView();
		}

		private static void OnGraphicsCreate(object sender, EventArgs e)
		{
			UpdateView();
		}

		private static void OnClientSizeChanged(object sender, EventArgs e)
		{
			if (LuxGame.Instance.Window.ClientBounds.Width > 0 && LuxGame.Instance.Window.ClientBounds.Height > 0 && !resizing)
			{
				resizing = true;

				_graphicsManager.PreferredBackBufferWidth = LuxGame.Instance.Window.ClientBounds.Width;
				_graphicsManager.PreferredBackBufferHeight = LuxGame.Instance.Window.ClientBounds.Height;
				UpdateView();

				resizing = false;
			}
		}

		/// <summary>
		/// Update the game's Viewport and ScreenMatrix
		/// </summary>
		private static void UpdateView()
		{
			float screenWidth = Width;
			float screenHeight = Height;

			int viewWidth;
			int viewHeight;

            // TODO: Width is actually VWidth down below. Support it again


			// Get view size
			if (screenWidth / Width > screenHeight / Height)
			{
				viewWidth = (int)(screenHeight / Height * Width);
				viewHeight = (int)screenHeight;
			}
			else
			{
				viewWidth = (int)screenWidth;
				viewHeight = (int)(screenWidth / Width * Height);
			}

			// Apply view padding
			float aspect = viewHeight / (float)viewWidth;
			viewWidth -= ViewPadding * 2;
			viewHeight -= (int)(aspect * ViewPadding * 2);

			// Update screen matrix
			float scale = viewWidth / (float)Width;
			ScreenMatrix = Matrix.CreateScale(scale, scale, 0f);

			// Update viewport
			Viewport = new Viewport
			{
				X = (int)(screenWidth / 2 - viewWidth / 2),
				Y = (int)(screenHeight / 2 - viewHeight / 2),
				Width = viewWidth,
				Height = viewHeight,
				MinDepth = 0f,
				MaxDepth = 1f
			};
		}
	}
}
