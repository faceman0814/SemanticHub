// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure.AI.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat
{
    public record ZhiPuChatResponseWrapper
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
        [JsonPropertyName("created")]
        public long Created { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

    }

    public record Choice
    {
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("message")]
        public Message Message { get; set; }
        [JsonPropertyName("delta")]
        public Delta Delta { get; set; }
    }

    public record Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    //public record ZhiPuUsage
    //{
    //    [JsonPropertyName("completion_tokens")]
    //    public int CompletionTokens { get; set; }
    //    [JsonPropertyName("prompt_tokens")]
    //    public int PromptTokens { get; set; }
    //    [JsonPropertyName("total_tokens")]
    //    public int TotalTokens { get; set; }
    //}
    public record Delta
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
