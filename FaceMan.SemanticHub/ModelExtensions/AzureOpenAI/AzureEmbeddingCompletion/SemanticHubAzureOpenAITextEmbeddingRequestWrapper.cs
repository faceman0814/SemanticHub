// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Yoyo.Pro.SemanticKernel.ModelExtensions.Azure.Chat;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion
{
    public record SemanticHubAzureOpenAITextEmbeddingRequestWrapper
    {
        [JsonPropertyName("input")]
        public string Input { get; init; }
        public static SemanticHubAzureOpenAITextEmbeddingRequestWrapper Create(string Input) => new()
        {
            Input = Input
        };
    }
}
