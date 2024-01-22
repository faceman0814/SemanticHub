using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;

using System.Text.Json.Serialization;

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
        public Usage Usage { get; set; }
    }
}
