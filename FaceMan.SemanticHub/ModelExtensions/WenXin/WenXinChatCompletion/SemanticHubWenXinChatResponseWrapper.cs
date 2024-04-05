// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;

using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin.Chat
{
    public record SemanticHubWenXinChatResponseWrapper
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("result")]
        public string Result { get; set; }
        [JsonPropertyName("is_truncated")]
        public bool IsTruncated { get; set; }
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory { get; set; }
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    //public record WenXinUsage
    //   {
    //	[JsonPropertyName("prompt_tokens")]
    //	public int PromptTokens { get; set; }
    //	[JsonPropertyName("completion_tokens")]
    //	public int CompletionTokens { get; set; }
    //	[JsonPropertyName("total_tokens")]
    //	public int TotalTokens { get; set; }
    //}
}
