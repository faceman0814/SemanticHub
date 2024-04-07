// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin.Chat
{
    public class SemanticHubWenXinChatCompletionService : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubWenXinConfig _config;
        private readonly ModelClient client;
        private readonly ILogger _log;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();
        public SemanticHubWenXinChatCompletionService(SemanticHubWenXinConfig config, ILoggerFactory? loggerFactory = null)
        {
            _config = config;
            client = new(config.Secret, ModelType.WenXin, config.Endpoint);
            _log = loggerFactory?.CreateLogger(typeof(SemanticHubAzureOpenAIChatCompletionService));
        }
        public async Task<(List<ChatMessage>, ChatParameters)> Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null, bool IsStream = false)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
            var histroyList = new List<ChatMessage>();
            var token = await client.WenXin.GetAccessToken(_config.ApiKey, _config.Secret);
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

            var isOnlyOne = chatHistory.Count == 1;
            foreach (var item in chatHistory)
            {
                if (item.Role == AuthorRole.System && !isOnlyOne)
                {
                    chatParameters.System = item.Content;
                }
                var history = new ChatMessage()
                {
                    Role = isOnlyOne ? "user" : item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }


            return (histroyList, chatParameters);
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = await Init(executionSettings, chatHistory);
            SemanticHubWenXinChatResponseWrapper response = await client.WenXin.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<ChatMessageContent>()
            {
                new ChatMessageContent(AuthorRole.Assistant, response.Result, metadata: metadata)
            };
            return result;
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = await Init(executionSettings, chatHistroy);
            SemanticHubWenXinChatResponseWrapper response = await client.WenXin.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<TextContent>()
            {
                new TextContent( response.Result, metadata: metadata)
            };
            return result;
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = await Init(executionSettings, chatHistroy, true);
            //返回流式聊天消息内容
            await foreach (var item in client.WenXin.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Result, metadata: metadata);
                yield return result;
            }
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = await Init(executionSettings, chatHistory, IsStream: true);
            //返回流式聊天消息内容
            await foreach (var item in client.WenXin.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Result, metadata: metadata);
                yield return result;
            }
        }

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubWenXinChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(6)
            {
                { nameof(completions.Id), completions.Id },
                { nameof(completions.Result), completions.Result },
                { nameof(completions.IsTruncated), completions.IsTruncated },
                { nameof(completions.NeedClearHistory), completions.NeedClearHistory },
                { nameof(completions.Usage), completions.Usage },
                { "Type", "WenXin" }
            };
        }
    }
}
