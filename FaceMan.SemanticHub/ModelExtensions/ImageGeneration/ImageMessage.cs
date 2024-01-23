using Json.Schema.Generation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.ImageGeneration
{
    public record ImageMessage
    {
        /// <summary>
        /// 指明需要调用的模型，固定值wanx-v1.
        /// </summary>
        [Required]
        [JsonPropertyName("model")]
        public string Model { get; set; } = "wanx-v1";
        /// <summary>
        /// 文本内容，支持中英文，中文不超过75个字，英文不超过75个单词，超过部分会自动截断。
        /// </summary>
        [Required]
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        /// <summary>
        /// 不生成的prompt信息
        /// </summary>
        [JsonPropertyName("negative_prompt")]
        public string NegativePrompt { get; set; }
    }
}
