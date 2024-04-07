// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin.Chat
{
    public record SemanticHubWenXinChatRequestWrapper
    {
        public static WenXinRequestWrapper<TMessages> Create<TMessages>(TMessages messages, ChatParameters parameters = default) => new()
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
            Stream = parameters != null ? parameters.Stream : false,
            Temperature = parameters != null && parameters.Temperature != null ? parameters.Temperature : default,
            TopP = parameters != null && parameters.TopP != null ? parameters.TopP : default,
            Stop = parameters != null && parameters.Stop != null ? parameters.Stop : default,
            PenaltyScore = parameters.PenaltyScore,
            System = parameters != null ? parameters.System : default,
            EnableCitation = parameters != null ? parameters.EnableCitation : false,
            DisableSearch = parameters != null ? parameters.DisableSearch : false,
            MaxOutputTokens = parameters != null ? parameters.MaxOutputTokens : 512,
        };

        public static WenXinRequestWrapper<TMessages> Create<TMessages>(TMessages messages) => new()
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
        };
    }

    public record WenXinRequestWrapper<TMessages> : SemanticHubWenXinChatRequestWrapper
    {
        /// <summary>
        /// 调用语言模型时，将当前对话信息列表作为提示输入给模型， 按照 {"role": "user", "content": "你好"} 的json 数组形式进行传参； 
        /// 可能的消息类型包括 System message、User message、Assistant message 和 Tool message。
        /// </summary>
        [JsonPropertyName("messages")]
        public TMessages Messages { get; init; }

        /// <summary>
        /// 说明：（1）较高的数值会使输出更加随机，而较低的数值会使其更加集中和确定。
        /// （2）默认0.95，范围 (0, 1.0]，不能为0。
        /// （3）建议该参数和top_p只设置1个。
        /// （4）建议top_p和temperature不要同时更改。
        /// </summary>
        [JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// 说明：（1）影响输出文本的多样性，取值越大，生成文本的多样性越强。
        /// （2）默认0.8，取值范围 [0, 1.0]。
        /// （3）建议该参数和temperature只设置1个。
        /// （4）建议top_p和temperature不要同时更改。
        /// </summary>
        [JsonPropertyName("top_p")]
        public float? TopP { get; set; }

        /// <summary>
        /// 通过对已生成的token增加惩罚，减少重复生成的现象。
        /// 说明：（1）值越大表示惩罚越大。
        /// （2）默认1.0，取值范围：[1.0, 2.0]。
        /// </summary>
        [JsonPropertyName("penalty_score")]
        public float? PenaltyScore { get; set; }

        /// <summary>
        /// 使用同步调用时，此参数应当设置为 fasle 或者省略。表示模型生成完所有内容后一次性返回所有内容。
        /// 如果设置为 true，模型将通过标准 Event Stream ，逐块返回模型生成内容。
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        /// <summary>
        /// 模型在遇到stop所制定的字符时将停止生成，目前仅支持单个停止词，格式为["stop_word1"]
        /// </summary>
        [JsonPropertyName("stop")]
        public object Stop { get; set; }

        /// <summary>
        /// 强制关闭实时搜索功能，默认false，表示不关闭
        /// </summary>
        [JsonPropertyName("disable_search")]
        public bool DisableSearch { get; set; }

        /// <summary>
        /// 是否开启上角标返回
        /// </summary>
        [JsonPropertyName("enable_citation")]
        public bool EnableCitation { get; set; }

        /// <summary>
        /// 模型人设，主要用于人设设定。
        /// 例如，你是xxx公司制作的AI助手，
        /// 说明：（1）长度限制1024个字符
        /// （2）如果使用functions参数，不支持设定人设system
        /// </summary>
        [JsonPropertyName("system")]
        public string System { get; set; }

        /// <summary>
        /// 单次限制最大token
        /// </summary>
        [JsonPropertyName("max_output_tokens")]
        public int? MaxOutputTokens { get; set; }
    }
}
