// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.Generation.TextGeneration
{
    public record ChatParameters
    {
        /// <summary>
        /// 结果的格式-“text”为旧文本版本，“message”为OpenAI兼容消息。
        /// ＜para＞对于语言模型，此字段必须是中的“text”，而不是VL模型中使用的字段</para>
        /// </summary>
        [JsonPropertyName("result_format")]
        public string ResultFormat { get; set; }

        /// <summary>
        /// 随机数生成器的种子，用于控制模型生成的随机性。
        /// 使用相同的种子允许模型输出的再现性。
        /// <para>此字段为可选字段。默认值为1234。</para>
        /// </summary>
        [JsonPropertyName("seed")]
        public ulong? Seed { get; set; }

        /// <summary>
        /// 限制要生成的令牌数量。限制设置了最大值，但不能保证
        /// 确切地说，将生成那么多令牌。此字段是可选的。
        /// <para>qwen turbo和qwen max longcontext的最大值和默认值为1500。</para>
        /// <para>qwen max、qwen-max-1201和qwen plus的最大值和默认值为2048。</para>
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 细胞核取样的概率阈值。以0.8的值为例，
        /// 仅保留累积概率总和大于或等于0.8的令牌。
        /// <para>取值范围为（0,1.0）。取值越大，随机性越高</para>
        /// <para>值越小，随机性越低。此字段是可选的.</para>
        /// <para>默认值为0.8。请注意，该值不应大于或等于1.</para>
        /// </summary>
        [JsonPropertyName("top_p")]
        public float? TopP { get; set; }

        /// <summary>
        /// 要采样的候选集的大小。例如，当设置为50时，只有前50个令牌
        /// 将考虑进行采样。此字段是可选的。较大的值会增加随机性；
        /// 较小的值会增加确定性。注意：如果top_ k为null或大于100，
        /// 没有使用topk策略，只有topp是有效的。默认值为null。
        /// </summary>
        [JsonPropertyName("top_k")]
        public int? TopK { get; set; }

        /// <summary>
        /// 为减少模型生成中的冗余而应用重复的惩罚。
        /// 值为1.0表示没有惩罚。此字段是可选的。
        /// <para>默认值为1.1。</para>
        /// </summary>
        [JsonPropertyName("repetition_penalty")]
        public float? RepetitionPenalty { get; set; }

        /// <summary>
        /// 控制文本生成的随机性和多样性程度。
        /// 高温度值会降低概率分布的峰值、
        /// 允许选择更多低概率词，从而产生更多样化的输出。
        /// <para>
        /// 低温度值会增加峰度，使高概率词更有可能被选中、
        /// 从而使输出结果更加确定。此字段为可选项。
        /// 数值范围为 [0, 2)。系统默认值为 1.0。
        /// </para>
        /// </summary>
        [JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// 指定生成后应停止模型进一步输出的内容。
        /// <para>这可以是一个字符串或字符串列表、一个标记 ID 列表或一个标记 ID 列表。
        /// <para>例如，如果将 stop 设置为 "hello"，则在生成 "hello "之前停止生成；</para
        /// <para>如果设置为[37763, 367]，则在生成相当于 "Observation "的标记 ID 之前停止生成。
        /// <para>
        /// 注意，此字段为可选字段，列表模式不支持字符串和标记 ID 混合使用；</para> <para>
        /// /// 注意，此字段为可选字段，列表模式不支持字符串和令牌 ID 混合使用。
        /// </para>
        /// </summary>
        [JsonPropertyName("stop")]
        public object Stop { get; set; }

        ///<summary>
        ///控制在生成过程中是否考虑搜索结果。
        ///＜para＞注意：启用搜索并不保证会使用搜索结果</para>
        ///<para>
        ///如果启用了搜索，则模型会将搜索结果视为提示的一部分以潜在地生成包括结果的文本。
        ///</para>
        ///＜para＞此字段为可选字段，默认为false</段落>
        ///</summary>
        [JsonPropertyName("enable_search")]
        public bool? EnableSearch { get; set; }

        /// <summary>
        ///控制是否启用增量输出模式。
        ///<para>
        ///默认值为false，这意味着后续输出将包含已完成的段。
        ///当设置为true时，将激活增量输出模式，并且后续输出将不包含
        ///之前的片段。完整的输出将需要由用户逐步构建。
        ///</para>
        ///此字段是可选的，仅适用于流输出模式。
        /// </summary>
        [JsonPropertyName("incremental_output")]
        public bool? IncrementalOutput { get; set; }

        /// <summary>
        /// 使用同步调用时，此参数应当设置为 fasle 或者省略。表示模型生成完所有内容后一次性返回所有内容。
        /// 如果设置为 true，模型将通过标准 Event Stream ，逐块返回模型生成内容。Event Stream 结束时会返回一条data: [DONE] 消息。
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        /// <summary>
        /// 智谱： do_sample 为 true 时启用采样策略，do_sample 为 false 时采样策略 temperature、top_p 将不生效
        /// </summary>
        [JsonPropertyName("do_sample")]
        public bool DoSample { get; set; }

        /// <summary>
        /// 文心
        /// 通过对已生成的token增加惩罚，减少重复生成的现象。
        /// 说明：（1）值越大表示惩罚越大。
        /// （2）默认1.0，取值范围：[1.0, 2.0]。
        /// </summary>
        [JsonPropertyName("penalty_score")]
        public float? PenaltyScore { get; set; }

        /// <summary>
        /// 文心
        /// 模型人设，主要用于人设设定。
        /// 例如，你是xxx公司制作的AI助手，
        /// 说明：（1）长度限制1024个字符
        /// （2）如果使用functions参数，不支持设定人设system
        /// </summary>
        [JsonPropertyName("system")]
        public string System { get; set; }

        /// <summary>
        /// 文心
        /// 强制关闭实时搜索功能，默认false，表示不关闭
        /// </summary>
        [JsonPropertyName("disable_search")]
        public bool DisableSearch { get; set; }

        /// <summary>
        /// 文心
        /// 是否开启上角标返回
        /// </summary>
        [JsonPropertyName("enable_citation")]
        public bool EnableCitation { get; set; }

        /// <summary>
        /// 文心
        /// 鉴权参数
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("presence_penalty")]
        public float? PresencePenalty { get; set; }

        [JsonPropertyName("frequency_penalty")]
        public float? FrequencyPenalty { get; set; }

        /// <summary>
        /// 文心
        /// 单次限制最大token
        /// </summary>
        [JsonPropertyName("max_output_tokens")]
        public int? MaxOutputTokens { get; set; }
    }
}
