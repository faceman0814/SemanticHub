// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions
{
    public interface IModelExtensionsChatCompletionService
    {
        /// <summary>
        /// 对话
        /// </summary>
        /// <param name="chatHistory">对话历史</param>
        /// <param name="settings">参数配置</param>
        /// <param name="kernel">SK的kernel</param>
        /// <param name="cancellationToken">是否取消</param>
        /// <returns></returns>
        Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 流式对话
        /// </summary>
        /// <param name="chatHistory">对话历史</param>
        /// <param name="settings">参数配置</param>
        /// <param name="kernel">SK的kernel</param>
        /// <param name="cancellationToken">是否取消</param>
        /// <returns></returns>
        IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
    }
}
