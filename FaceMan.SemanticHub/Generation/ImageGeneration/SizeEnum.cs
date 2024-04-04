using FaceMan.SemanticHub.Helper.EnumHelper;

using System.ComponentModel;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public enum SizeEnum
    {
        /// <summary>
        /// 1024*1024
        /// </summary>
        [Description("1024x1024")]
        [EnumName("1024x1024")]
        Size1024x1024 = 0,

        /// <summary>
        /// 256x256
        /// </summary>
        [Description("256x256")]
        [EnumName("256x256")]
        Size256x256,

        /// <summary>
        /// 512x512
        /// </summary>
        [Description("512x512")]
        [EnumName("512x512")]
        Size512x512,

        /// <summary>
        /// 1024x1792
        /// </summary>
        [Description("1024x1792")]
        [EnumName("1024x1792")]
        Size1024x1792,

        /// <summary>
        /// 1792x1024
        /// </summary>
        [Description("1792x1024")]
        [EnumName("1792x1024")]
        Size1792x1024
    }
}
