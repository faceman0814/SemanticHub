// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Chat
{
    public class SemanticHubTongYiChatCompletionService : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubTongYiConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();
        private readonly ILogger _log;
        public SemanticHubTongYiChatCompletionService(SemanticHubTongYiConfig config, ILoggerFactory? loggerFactory = null)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.TongYi, config.Endpoint);
            _log = loggerFactory?.CreateLogger(typeof(SemanticHubTongYiChatCompletionService));
        }

        (List<ChatMessage>, ChatParameters) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = new ChatParameters()
            {
                TopP = settings != null && settings.TopP != 1 ? (float)settings.TopP : (float)0.75,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)0.95,
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

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            SemanticHubTongYiChatResponseWrapper response = await client.TongYi.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<ChatMessageContent>()
            {
                new ChatMessageContent(AuthorRole.Assistant, response.Output.Text, metadata: metadata)
            };
            return result;
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy);
            SemanticHubTongYiChatResponseWrapper response = await client.TongYi.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<TextContent>()
            {
                new TextContent( response.Output.Text, metadata: metadata)
            };
            return result;
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy);
            //返回流式聊天消息内容
            await foreach (var item in client.TongYi.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Output.Text, metadata: metadata);
                yield return result;
            }
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            //返回流式聊天消息内容
            await foreach (var item in client.TongYi.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Output.Text, metadata: metadata);
                yield return result;
            }
        }

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubTongYiChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(4)
            {
                { nameof(completions.Output), completions.Output },
                { nameof(completions.RequestId), completions.RequestId },
                { nameof(completions.Usage), completions.Usage },
                { "Type", "TongYi" }
            };
        }
    }

}
