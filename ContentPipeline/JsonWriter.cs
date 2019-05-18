/*
 * Author: Shon Verch
 * File Name: JsonWriter.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/19
 * Modified Date: 05/18/19
 * Description: The content type writer for JSON file data.
 */

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace FreeCell.ContentPipeline
{
    /// <inheritdoc />
    /// <summary>
    /// The content type writer for <see cref="T:FreeCell.ContentPipeline.JsonFileData" />.
    /// </summary>
    [ContentTypeWriter]
    public class JsonWriter : ContentTypeWriter<JsonFileData>
    {
        /// <inheritdoc />
        /// <summary>
        /// Retrieves the assembly name of the content type reader for JSON data.
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform) =>
            typeof(JsonReader).AssemblyQualifiedName;

        /// <inheritdoc />
        /// <summary>
        /// Writes the <see cref="T:FreeCell.ContentPipeline.JsonFileData" /> into a content file.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="data"></param>
        protected override void Write(ContentWriter output, JsonFileData data)
        {
            output.Write(data.JsonData);
        }
    }
}
