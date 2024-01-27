using FaceMan.SemanticHub.Generation.ImageGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ConverterHelper
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
