/*
 * Author: Shon Verch
 * File Name: TextureAtlas.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/19
 * Modified Date: 05/18/19
 * Description: A texture atlas is a large texture that contains many smaller
 *              textures within it. This class provides a clean interface for
 *              loading in a texture atlas and extracting data from it.
 */

using System.Collections.Generic;
using System.IO;
using FreeCell.ContentPipeline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using MonoGameUtilities.Logging;
using Newtonsoft.Json;

namespace FreeCell
{
    /// <summary>
    /// A single entry in the texture atlas.
    /// </summary>
    public struct TextureAtlasEntry
    {

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("rectangle", Required = Required.Always)]
        public Rectangle Rectangle { get; set; }
    }

    /// <summary>
    /// A texture atlas is a large texture that contains many smaller
    /// textures within it.This class provides a clean interface for
    /// loading in a texture atlas and extracting data from it.
    /// </summary>
    public class TextureAtlas
    {
        /// <summary>
        /// Gets a <see cref="Texture2D"/> by name.
        /// </summary>
        public Texture2D this[string name] => Get(name);

        private readonly Dictionary<string, Texture2D> textureAtlasEntries;

        /// <summary>
        /// Initializes a new <see cref="TextureAtlas"/>.
        /// </summary>
        /// <param name="atlasContentFilePath">The file path to the texture atlas in the content.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="contentManager">The content manager.</param>
        public TextureAtlas(string atlasContentFilePath, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            textureAtlasEntries = new Dictionary<string, Texture2D>();
            Texture2D textureAtlas = contentManager.Load<Texture2D>(atlasContentFilePath);

            string metaFilepath = Path.ChangeExtension(atlasContentFilePath, "meta");
            TextureAtlasEntry[] metaDataEntries = contentManager.Load<JsonObject>(metaFilepath).Convert<TextureAtlasEntry[]>();
            foreach (TextureAtlasEntry entry in metaDataEntries)
            {
                textureAtlasEntries[entry.Name] = TextureHelpers.GetCroppedTexture(textureAtlas, entry.Rectangle, graphicsDevice);
            }

            // Since we have load all the textures from our atlas, there is no
            // reason to keep the texture atlas data allocated; hence, we can unload it.
            contentManager.Unload();
        }

        /// <summary>
        /// Gets a <see cref="Texture2D"/> by name.
        /// </summary>
        public Texture2D Get(string name)
        {
            if (textureAtlasEntries.ContainsKey(name)) return textureAtlasEntries[name];

            Logger.LogFunctionEntry(string.Empty, $"Could not find texture with name \"{name}\".", LoggerVerbosity.Warning);
            return null;
        }
    }
}
