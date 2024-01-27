// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Chat
{
    /// <summary>
    /// 请求包装器
    /// </summary>
    public record TongYiChatRequestWrapper
    {
        public static QianWenRequestWrapper<TMessages, TParameters> Create<TMessages, TParameters>(string model, TMessages messages, TParameters? parameters = default) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            Messages = messages ?? throw new ArgumentNullException(nameof(messages)),
            Parameters = parameters,
        };

        public static QianWenRequestWrapper<TMessages, object> Create<TMessages>(string model, TMessages inputPrompt) => new()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model)),
            Messages = inputPrompt ?? throw new ArgumentNullException(nameof(inputPrompt)),
        };
    }

    public record QianWenRequestWrapper<TMessages, TParameters> : TongYiChatRequestWrapper
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("input")]
        public TMessages Messages { get; init; }

        [JsonPropertyName("parameters")]
        public TParameters? Parameters { get; init; }
    }
}
