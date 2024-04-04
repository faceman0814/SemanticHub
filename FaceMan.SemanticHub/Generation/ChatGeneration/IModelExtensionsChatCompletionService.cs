// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.Generation.ChatGeneration
{
    public interface IModelExtensionsChatCompletionService
    {
        /// <summary>
        /// 对话————返回token使用情况
        /// </summary>
        /// <param name="chatHistory">对话历史</param>
        /// <param name="settings">参数配置</param>
        /// <param name="kernel">SK的kernel</param>
        /// <param name="cancellationToken">是否取消</param>
        /// <returns></returns>
        Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 流式对话————返回token使用情况
        /// </summary>
        /// <param name="chatHistory">对话历史</param>
        /// <param name="settings">参数配置</param>
        /// <param name="kernel">SK的kernel</param>
        /// <param name="cancellationToken">是否取消</param>
        /// <returns></returns>
        IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);/// <summary>

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
