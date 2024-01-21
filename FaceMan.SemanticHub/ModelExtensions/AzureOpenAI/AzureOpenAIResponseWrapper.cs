using Azure.AI.OpenAI;

using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI
{
    public record AzureOpenAIResponseWrapper
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

    public record Choice
    {
        [JsonPropertyName("finish_details")]
        public FinishDetails FinishDetails { get; set; }
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("message")]
        public Message Message { get; set; }
        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
        [JsonPropertyName("delta")]
        public Delta Delta { get; set; }
    }

    public record PromptFilterResults
    {
        [JsonPropertyName("prompt_index")]
        public int PromptIndex { get; set; }
        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }

    public record Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public record ContentFilterResults
    {
        [JsonPropertyName("hate")]
        public FilterResults Hate { get; set; }
        [JsonPropertyName("self_harm")]
        public FilterResults SelfHarm { get; set; }
        [JsonPropertyName("sexual")]
        public FilterResults Sexual { get; set; }
        [JsonPropertyName("violence")]
        public FilterResults Violence { get; set; }
    }

    public record FinishDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("stop")]
        public string Stop { get; set; }
    }

    public record FilterResults
    {
        [JsonPropertyName("filtered")]
        public bool Filtered { get; set; }
        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }

    public record Delta
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public record Usage
    {
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
