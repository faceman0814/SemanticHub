using FaceMan.SemanticHub.Helper.EnumHelper;

using System.ComponentModel;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public enum SizeEnum
    {
        /// <summary>
		/// 1024*1024
		/// </summary>
		[Description("1024*1024")]
        [EnumName("1024*1024")]
        Size1024x1024 = 0,

        /// <summary>
        /// 720*1280
        /// </summary>
        [Description("720*1280")]
        [EnumName("720*1280")]
        Size720x1280,

        /// <summary>
        /// 1280*720
        /// </summary>
        [Description("1280*720")]
        [EnumName("1280*720")]
        Size1280x720
    }
}
