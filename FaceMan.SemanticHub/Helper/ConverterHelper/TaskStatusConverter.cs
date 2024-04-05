using FaceMan.SemanticHub.Generation.ImageGeneration;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.Helper.ConverterHelper
{
    public class TaskStatusConverter : JsonConverter<ImageTaskStatusEnum>
    {
        /// <inheritdoc/>
        public override ImageTaskStatusEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            if (Enum.TryParse(value, true, out ImageTaskStatusEnum status))
            {
                return status;
            }
            throw new JsonException($"Invalid value '{value}' for {nameof(ImageTaskStatusEnum)}.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ImageTaskStatusEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
