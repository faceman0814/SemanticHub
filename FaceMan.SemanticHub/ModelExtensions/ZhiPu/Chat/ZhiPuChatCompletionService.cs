// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.TextGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat
{
    public class ZhiPuChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly string _model;
        private readonly ModelClient client;
        public ZhiPuChatCompletionService(string secret, string model, string url = null)
        {
            _model = model;
            client = new(secret, ModelType.ZhiPu, url);
        }

        (List<ChatMessage>, ChatParameters) Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, bool isStream = false)
        {
            var histroyList = new List<ChatMessage>();
            //因为智谱AI官方调用的有bug，所以这里做一下处理。
            histroyList.Add(ChatMessage.FromSystem("1"));
            ChatParameters chatParameters = new ChatParameters()
            {
                TopP = settings != null && settings.Temperature != 1 ? (float)settings.TopP : (float)0.75,
                // max_tokens 应该在 [1, 1500]的区间
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null && settings.Temperature != 1 ? (float)settings.Temperature : (float)0.95,
            };
            foreach (var item in chatHistory)
            {
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            return (histroyList, chatParameters);
        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            var result = await client.ZhiPu.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            return new ChatMessageContent(AuthorRole.Assistant, result.Choices[0].Message.Content);
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            var result = await client.ZhiPu.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            return (new ChatMessageContent(AuthorRole.Assistant, result.Choices[0].Message.Content), result.Usage);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings, true);

            await foreach (var item in client.ZhiPu.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return item.Item1;
            }
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings, true);
            await foreach (var item in client.ZhiPu.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return (item.Item1, item.Item2);
            }
        }
    }
}
