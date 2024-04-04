// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion
{
    public record SemanticHubAzureOpenAITextEmbeddingResponseWrapper
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("data")]
        public List<Data> Data { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("usage")]
        public TextEmbeddingUsage Usage { get; set; }
    }
    public record Data
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("embedding")]
        public List<float> Embedding { get; set; }
    }

    public record TextEmbeddingUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
