// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
    /// <summary>
    /// 用于映像请求异步任务的通用基本响应类。
    /// </summary>
    public record QianWenResponseWrapper
    {
        /// <summary>
        /// The identifier corresponds to each individual request.
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; init; }

        /// <summary>
        /// The processed task status response associated with the respective request.
        /// </summary>
        [JsonPropertyName("output")]
        public Output Output { get; init; }

        /// <summary>
        /// Usage of the request.
        /// </summary>
        [JsonPropertyName("usage")]
        public QianWenUsage? Usage { get; init; }
    }

    /// <summary>
    /// 聊天请求的令牌使用情况。
    /// </summary>
    public record QianWenUsage
    {
        /// <summary>
        /// 输出消息的令牌计数。
        /// </summary>
        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; init; }

        /// <summary>
        /// 输入消息的令牌计数。
        /// </summary>
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; init; }
        /// <summary>
        /// 总令牌计数。
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; init; }
    }

    /// <summary>
    /// 聊天请求的输出。
    /// </summary>
    public record Output
    {
        /// <summary>
        /// 模型的输出内容。
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; init; }

        /// <summary>
        /// 有3种情况：
        /// <list type="bullet">
        /// <item><c>null</c>正在生成</item>
        /// <item><c>stop</c> 停止了</item>
        /// <item><c>length</c> 文本太长</item>
        /// </list>
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; init; }
    }
}
