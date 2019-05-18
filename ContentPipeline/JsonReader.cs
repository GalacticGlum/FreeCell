/*
 * Author: Shon Verch
 * File Name: JsonReader.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/19
 * Modified Date: 05/18/19
 * Description: The content reader for JsonObjects.
 */

using Microsoft.Xna.Framework.Content;

namespace FreeCell.ContentPipeline
{
    /// <summary>
    /// The content reader for <see cref="JsonObject"/>s.
    /// </summary>
    public class JsonReader : ContentTypeReader<JsonObject>
    {
        /// <summary>
        /// Reads a <see cref="JsonObject"/>.
        /// </summary>
        protected override JsonObject Read(ContentReader input, JsonObject existingInstance)
        {
            string json = input.ReadString();
            return new JsonObject(json);
        }
    }
}
