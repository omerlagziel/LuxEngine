using System;
namespace Lux.Framework
{
    public static class HardCodedConfig
    {
        // Network
        public const int PROTOCOL_VERSION = 1;

        // Ticks
        public const int TICKS_PER_SECOND = 1;
        //public const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
        //public const int MAX_FRAMESKIP = 5; // todo ?

        // Data
        public const int MAX_ENTITIES_PER_WORLD = 2048;
        public const int MAX_GAME_COMPONENT_TYPES = 64;
        public const int MAX_SYSTEMS = 128;

        // Assets
        public const string DEFAULT_TEXTURES_FOLDER_NAME = "Textures";
        public const string DEFAULT_MAPS_FOLDER_NAME = "Maps";
        public const string DEFAULT_TILESETS_FOLDER_NAME = "Tilesets";

        // Preprocessor definitions

        /// <summary>
        /// Defined when the system is NOT a console
        /// </summary>
        public const string NCONSOLE = "NCONSOLE";
    }
}
