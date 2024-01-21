using FaceMan.SemanticHub.ModelExtensions.Azure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI
{
    public record OpenAIResponseWrapper
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("prompt_filter_results")]
        public List<PromptFilterResults> PromptFilterResults { get; set; }
        [JsonPropertyName("usage")]
        public Usage usage { get; set; }
    }
}
