using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lux.Framework
{
    /// <summary>
    /// Custom content manager for loading, reloading and disposing assets.
    /// </summary>
    public class LuxContentManager : ContentManager
    {
		//private Dictionary<string, Effect> _loadedEffects = new Dictionary<string, Effect>();

		private List<IDisposable> _disposableAssets;

		private List<IDisposable> DisposableAssets
		{
			get
			{
				if (_disposableAssets == null)
				{
					var fieldInfo = ReflectionUtils.GetFieldInfo(typeof(ContentManager), "disposableAssets");
					_disposableAssets = fieldInfo.GetValue(this) as List<IDisposable>;
				}

				return _disposableAssets;
			}
		}

#if FNA
		private Dictionary<string, object> _loadedAssets;
		private Dictionary<string, object> LoadedAssets
		{
			get
			{
				if (_loadedAssets == null)
				{
					var fieldInfo = ReflectionUtils.GetFieldInfo(typeof(ContentManager), "loadedAssets");
					_loadedAssets = fieldInfo.GetValue(this) as Dictionary<string, object>;
				}
				return _loadedAssets;
			}
		}
#endif

		public LuxContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        { }

        public LuxContentManager() : base(LuxGame.Instance.Services, ((Game)LuxGame.Instance).Content.RootDirectory)
        { }

        public Texture2D LoadTexture(string name)
        {
			if (LoadedAssets.TryGetValue(name, out var asset))
			{
				if (asset is Texture2D tex)
					return tex;
			}

			string pngPath = $"{LuxGame.ContentDirectory}/{HardCodedConfig.DEFAULT_TEXTURES_FOLDER_NAME}/{name}.png";
			using (var stream = Path.IsPathRooted(name) ? File.OpenRead(name) : TitleContainer.OpenStream(pngPath))
			{
				var texture = Texture2D.FromStream(LuxGame.GraphicsDevice, stream);
				texture.Name = name;
				LoadedAssets[name] = texture;
				DisposableAssets.Add(texture);

				return texture;
			}
        }
    }
}
