/*
 * Author: Shon Verch
 * File Name: JsonObject.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/19
 * Modified Date: 05/18/19
 * Description: A data object containing the result of the JsonReader.
 *              This type is used in Content.Load calls.
 */

using Newtonsoft.Json;

namespace FreeCell.ContentPipeline
{
    /// <summary>
    /// A data object containing the result of the <see cref="JsonReader"/>.
    /// This type is used in <c>Content.Load</c> calls.
    /// </summary>
    public class JsonObject
    {
        /// <summary>
        /// The soruce text of the JSON file.
        /// </summary>
        public string JsonSource { get; }

        /// <summary>
        /// Initializes a new <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="jsonSource"></param>
        public JsonObject(string jsonSource)
        {
            JsonSource = jsonSource;
        }
        
        /// <summary>
        /// Converts this <see cref="JsonObject"/> to an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to convrt to.</typeparam>
        /// <returns>An object of type <typeparamref name="T"/> representing this <see cref="JsonObject"/>.</returns>
        public T Convert<T>() => JsonConvert.DeserializeObject<T>(JsonSource);
    }
}
