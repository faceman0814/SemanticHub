using Json.Schema.Generation;

using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public record ImageParameters
    {
        /// <summary>
        /// 输出图像的风格
        /// </summary>
        public StyleEnum _imageStyle;

        /// <summary>
        /// 输出图像的风格
        /// </summary>
        public StyleEnum ImageStyle
        {
            get
            {
                return _imageStyle;
            }
            set
            {
                _imageStyle = value;
                Style = _imageStyle.ToNameValue();
            }
        }

        /// <summary>
        /// 模型供应商类型中文描述
        /// </summary>
        [JsonPropertyName("style")]
        public string Style { get; set; } = "<auto>";


        /// <summary>
        /// 生成图像的分辨率
        /// </summary>
        public SizeEnum _imageSize;

        /// <summary>
        /// 生成图像的分辨率
        /// </summary>
        public SizeEnum ImageSize
        {
            get
            {
                return _imageSize;
            }
            set
            {
                _imageSize = value;
                Size = _imageSize.ToNameValue();
            }
        }

        /// <summary>
        /// 生成图像的分辨率,默认1024*1024。
        /// </summary>
        [Required]
        [JsonPropertyName("size")]
        public string Size { get; set; } = "1024*1024";

        /// <summary>
        /// 本次请求生成的图片数量，目前支持1~4张，默认为1。
        /// </summary>
        [JsonPropertyName("n")]
        public int? n { get; set; } = 1;

        /// <summary>
        /// 图片生成时候的种子值，取值范围为(0,4294967290) 。
        /// 如果不提供，则算法自动用一个随机生成的数字作为种子，
        /// 如果给定了，则根据 batch 数量分别生成 seed, seed+1, seed+2, seed+3 为参数的图片。
        /// </summary>
        [JsonPropertyName("seed")]
        public int? seed { get; set; }

        /// <summary>
        /// 文心：鉴权token
        /// </summary>
        public string Token { get; set; }
    }
}
