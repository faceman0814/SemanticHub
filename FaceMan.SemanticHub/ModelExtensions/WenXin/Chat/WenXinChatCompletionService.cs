// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.EMMA;
using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Newtonsoft.Json;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin.Chat
{
    public class WenXinChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly string _secret;
        private readonly string _key;
        private readonly string _model;
        private readonly ModelClient client;
        public WenXinChatCompletionService(string key, string secret, string model, string url = null)
        {
            _secret = secret;
            _model = model;
            _key = key;
            client = new(secret, ModelType.WenXin, url);
        }
        async Task<(List<ChatMessage>, ChatParameters)> Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, bool IsStream = false)
        {
            var histroyList = new List<ChatMessage>();
            var token = await client.WenXin.GetAccessToken(_key, _secret);
            var system = chatHistory.FirstOrDefault(t => t.Role == AuthorRole.System);
            ChatParameters chatParameters = new ChatParameters()
            {
                TopP = settings != null ? (float)settings.TopP : (float)0.75,
                Temperature = settings != null ? (float)settings.Temperature : (float)0.95,
                System = system != null ? system.Content : default,
                MaxOutputTokens = settings != null ? settings.MaxTokens : 512,
                Token = token,
                Stream = IsStream
            };
            foreach (var item in chatHistory)
            {
                if (item.Role == AuthorRole.System)
                {
                    continue;
                }
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
            (var histroyList, var chatParameters) = await Init(chatHistory, settings);
            var result = await client.WenXin.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            return new ChatMessageContent(AuthorRole.Assistant, result.Result);
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = await Init(chatHistory, settings);
            var result = await client.WenXin.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            return (new ChatMessageContent(AuthorRole.Assistant, result.Result), result.Usage);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = await Init(chatHistory, settings, true);
            await foreach (var item in client.WenXin.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return item.Item1;
            }
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = await Init(chatHistory, settings, true);
            await foreach (var item in client.WenXin.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return (item.Item1, item.Item2);
            }
        }

    }
}
