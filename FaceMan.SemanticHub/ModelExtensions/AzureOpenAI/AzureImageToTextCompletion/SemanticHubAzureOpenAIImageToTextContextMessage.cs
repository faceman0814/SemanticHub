using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureImageToTextCompletion
{
    public record SemanticHubAzureOpenAIImageToTextContextMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<Content> Content { get; set; }
    }

    public record Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public record ImageUrl
    {
        [JsonPropertyName("image_url")]
        public string Url { get; set; }
    }
}
