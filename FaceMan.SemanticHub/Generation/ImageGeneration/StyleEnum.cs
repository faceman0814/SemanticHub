﻿using FaceMan.SemanticHub.Helper.EnumHelper;

using System.ComponentModel;

namespace FaceMan.SemanticHub.Generation.ImageGeneration
{
    public enum StyleEnum
    {
        /// <summary>
		/// 自动
		/// </summary>
		[Description("自动")]
        [EnumName("<auto>")]
        Auto = 0,

        /// <summary>
        /// 3d卡通
        /// </summary>
        [Description("3d卡通")]
        [EnumName("<3d cartoon>")]
        ThreeDCartoon,

        /// <summary>
        /// 动画
        /// </summary>
        [Description("动画")]
        [EnumName("<anime>")]
        Anime,

        /// <summary>
        /// 油画
        /// </summary>
        [Description("油画")]
        [EnumName("<oil painting>")]
        OilPainting,

        /// <summary>
        /// 水彩
        /// </summary>
        [Description("水彩")]
        [EnumName("<watercolor>")]
        Watercolor,

        /// <summary>
        /// 素描
        /// </summary>
        [Description("素描")]
        [EnumName("<sketch>")]
        Sketch,

        /// <summary>
        /// 中国画
        /// </summary>
        [Description("中国画")]
        [EnumName("<chinese painting>")]
        ChinesePainting,

        /// <summary>
        /// 扁平插画
        /// </summary>
        [Description("扁平插画")]
        [EnumName("<flat illustration>")]
        FlatIllustration,

        /// <summary>
		/// 生动
		/// </summary>
		[Description("生动")]
        [EnumName("vivid")]
        Vivid = 0,

        /// <summary>
        /// 自然
        /// </summary>
        [Description("自然")]
        [EnumName("natural")]
        Natural,
    }
}
