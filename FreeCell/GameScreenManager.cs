/*
 * Author: Shon Verch
 * File Name: GameScreenManager.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: Manages all the game screens.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// Manages all the <see cref="GameScreen"/>s.
    /// </summary>
    public class GameScreenManager
    {
        /// <summary>
        /// A mapping of all the <see cref="GameScreen"/> instances to their respective string identifiers.
        /// </summary>
        private readonly Dictionary<Type, GameScreen> gameScreens;

        /// <summary>
        /// The <see cref="SpriteBatch"/> instance.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The identifier of the active game screen.
        /// </summary>
        private Type activeGameScreenType;

        /// <summary>
        /// Initializes a new <see cref="GameScreenManager"/>.
        /// </summary>
        public GameScreenManager()
        {
            gameScreens = new Dictionary<Type, GameScreen>();

            // Find all game screen types
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // If type is not a child of GameScreen, we don't consider it.
                    if (type.BaseType != typeof(GameScreen)) continue;

                    GameScreen gameScreen = (GameScreen) Activator.CreateInstance(type);
                    gameScreens.Add(type, gameScreen);
                }
            }
        }

        /// <summary>
        /// Switch the active <see cref="GameScreen"/> to the <see cref="GameScreen"/>
        /// with the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="GameScreen"/>.</typeparam>
        public void SwitchScreen<T>() where T : GameScreen => SwitchScreen(typeof(T));

        /// <summary>
        /// Switch the active <see cref="GameScreen"/> to the <see cref="GameScreen"/>
        /// with the specified type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="GameScreen"/> to switch to.</param>
        public void SwitchScreen(Type type)
        {
            if (!gameScreens.ContainsKey(type))
            {
                Logger.LogFunctionEntry(string.Empty, "Attempted to switch to game screen of type that does not exist in the cache.", LoggerVerbosity.Warning);
                return;
            }

            activeGameScreenType = type;
            gameScreens[type].OnScreenStarted();
        }

        /// <summary>
        /// Reloads a <see cref="GameScreen"/> of the specified type <typeparamref name="T"/>.
        /// <remarks>
        /// This method reinitializes the <see cref="GameScreen"/> by calling its constructors.
        /// This will reset all data.
        /// </remarks>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="GameScreen"/>.</typeparam>
        public void ReloadScreen<T>() where T : GameScreen => ReloadScreen(typeof(T));

        /// <summary>
        /// Reload a <see cref="GameScreen"/> of the specified <paramref name="type"/>.
        /// <remarks>
        /// This method reinitializes the <see cref="GameScreen"/> by calling its constructors.
        /// This will reset all data.
        /// </remarks>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="GameScreen"/> to reload.</param>
        public void ReloadScreen(Type type)
        {
            if (!gameScreens.ContainsKey(type))
            {
                Logger.LogFunctionEntry(string.Empty, "Attempted to reload game screen of type that does not exist in the cache.", LoggerVerbosity.Warning);
                return;
            }

            gameScreens[type] = (GameScreen) Activator.CreateInstance(type);
            gameScreens[type].LoadContent(spriteBatch);
        }

        /// <summary>
        /// Retrieves the <see cref="GameScreen"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="GameScreen"/>.</typeparam>
        /// <returns>A <see cref="GameScreen"/> of type <typeparamref name="T"/> or <value>null</value> if it could not be found.</returns>
        public T Get<T>() where T : GameScreen => (T)Get(typeof(T));

        /// <summary>
        /// Retrieves the <see cref="GameScreen"/> of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="GameScreen"/> to get.</param>
        /// <returns>A <see cref="GameScreen"/> of <paramref name="type"/> or <value>null</value> if it could not be found.</returns>
        public GameScreen Get(Type type)
        {
            if (gameScreens.ContainsKey(type)) return gameScreens[type];

            Logger.LogFunctionEntry(string.Empty, "Attempted to get game screen of type that does not exist in the cache.", LoggerVerbosity.Warning);
            return null;

        }

        /// <summary>
        /// Calls LoadContent on all of the <see cref="GameScreen"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance passed to the <see cref="GameScreen"/>s.</param>
        public void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            foreach (KeyValuePair<Type, GameScreen> screenPair in gameScreens)
            {
                screenPair.Value.LoadContent(spriteBatch);
            }
        }

        /// <summary>
        /// Calls Update on the active <see cref="GameScreen"/>.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (activeGameScreenType == null) return;
            gameScreens[activeGameScreenType].Update(gameTime);
        }

        /// <summary>
        /// Calls Draw on the active <see cref="GameScreen"/>.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            if (activeGameScreenType == null) return;
            gameScreens[activeGameScreenType].Draw(gameTime);
        }
    }
}
