// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.Drawing.Charts;

using FaceMan.SemanticHub.Generation.ChatGeneration;

using Microsoft.Graph;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat
{
    /// 请求包装器
    /// </summary>
    public record SemanticHubZhiPuChatRequestWrapper
    {
        public static ZhiPuRequestWrapper<TMessages> Create<TMessages>(string model, TMessages messages, ChatParameters parameters = default) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
            MaxTokens = parameters != null ? parameters.MaxTokens : null,
            Stream = parameters != null ? parameters.Stream : false,
            Temperature = parameters != null ? parameters.Temperature : default,
            TopP = parameters != null ? parameters.TopP : default,
        };

        public static ZhiPuRequestWrapper<TMessages> Create<TMessages>(string model, TMessages messages) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
        };
    }

    public record ZhiPuRequestWrapper<TMessages> : SemanticHubZhiPuChatRequestWrapper
    {
        /// <summary>
        /// 所要调用的模型编码
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// 调用语言模型时，将当前对话信息列表作为提示输入给模型， 按照 {"role": "user", "content": "你好"} 的json 数组形式进行传参； 
        /// 可能的消息类型包括 System message、User message、Assistant message 和 Tool message。
        /// </summary>
        [JsonPropertyName("messages")]
        public TMessages Messages { get; init; }

        /// <summary>
        /// 模型输出最大 tokens
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 采样温度，控制输出的随机性，必须为正数取值范围是：(0.0, 1.0)，不能等于 0，默认值为 0.95，值越大，会使输出更随机，更具创造性；
        /// 值越小，输出会更加稳定或确定建议您根据应用场景调整 top_p 或 temperature 参数，但不要同时调整两个参数
        /// </summary>
        [JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// 用温度取样的另一种方法，称为核取样取值范围是：(0.0, 1.0) 开区间，不能等于 0 或 1，默认值为 0.7模型考虑具有 top_p 概率质量 tokens 的结果 
        /// 例如：0.1 意味着模型解码器只考虑从前 10% 的概率的候选集中取 tokens 建议您根据应用场景调整 top_p 或 temperature 参数，但不要同时调整两个参数
        /// </summary>
        [JsonPropertyName("top_p")]
        public float? TopP { get; set; }



        /// <summary>
        /// 使用同步调用时，此参数应当设置为 fasle 或者省略。表示模型生成完所有内容后一次性返回所有内容。
        /// 如果设置为 true，模型将通过标准 Event Stream ，逐块返回模型生成内容。
        /// Event Stream 结束时会返回一条data: [DONE] 消息。
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        ///// <summary>
        ///// 模型在遇到stop所制定的字符时将停止生成，目前仅支持单个停止词，格式为["stop_word1"]
        ///// </summary>
        //[JsonPropertyName("stop")]
        //public object? Stop { get; set; }

        ///// <summary>
        ///// do_sample 为 true 时启用采样策略，do_sample 为 false 时采样策略 temperature、top_p 将不生效.
        ///// </summary>
        //[JsonPropertyName("do_sample")]
        //public bool DoSample { get; set; }
    }
}
