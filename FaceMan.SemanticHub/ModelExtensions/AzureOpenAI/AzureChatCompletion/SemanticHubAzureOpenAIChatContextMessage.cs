﻿using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion
{
    public record SemanticHubAzureOpenAIChatContextMessage
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
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    //public enum ContentType
    //{
    //    [Description("Text")]
    //    [EnumName("Text")]
    //    [JsonPropertyName("text")]
    //    Text = 1,

    //    [Description("ImageUrl")]
    //    [EnumName("ImageUrl")]
    //    [JsonPropertyName("image_url")]
    //    ImageUrl,
    //}
}
