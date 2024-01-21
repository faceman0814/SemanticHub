using DocumentFormat.OpenXml.EMMA;

using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI
{
    public record OpenAIRequestWrapper
    {
        public static OpenAIRequestWrapper<TMessages> Create<TMessages>(string model, TMessages messages, ChatParameters parameters = default) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
            MaxTokens = parameters != null ? parameters.MaxTokens : 1024,
            Stream = parameters != null ? parameters.Stream : false,
            Temperature = parameters != null ? parameters.Temperature : (float)0.7,
            TopP = parameters != null ? parameters.TopP : (float)0.95,
        };

        public static OpenAIRequestWrapper<TMessages> Create<TMessages>(TMessages messages) => new()
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages))
        };
    }

    public record OpenAIRequestWrapper<TMessages> : OpenAIRequestWrapper
    {
        /// <summary>
        /// 调用语言模型时，将当前对话信息列表作为提示输入给模型， 按照 {"role": "user", "content": "你好"} 的json 数组形式进行传参； 
        /// 可能的消息类型包括 System message、User message、Assistant message 和 Tool message。
        /// </summary>
        [JsonPropertyName("messages")]
        public TMessages Messages { get; init; }

        /// <summary>
        /// 大语言模型
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; init; }

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
    }
}
