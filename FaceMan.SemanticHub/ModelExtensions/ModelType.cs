// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.EnumHelper;

using System.ComponentModel;

namespace FaceMan.SemanticHub.ModelExtensions
{
    public enum ModelType
    {
        [Description("通义")]
        [EnumName("通义")]
        TongYi = 1,

        [Description("智谱AI")]
        [EnumName("智谱AI")]
        ZhiPu,

        [Description("科大讯飞")]
        [EnumName("科大讯飞")]
        XunFei,

        [Description("文心一言")]
        [EnumName("文心一言")]
        WenXin,

        [Description("AzureOpenAI")]
        [EnumName("AzureOpenAI")]
        AzureOpenAI,

        [Description("OpenAI")]
        [EnumName("OpenAI")]
        OpenAI,


    }
}
