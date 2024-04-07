// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat
{
    public class SemanticHubZhiPuChatCompletionService : ISemanticHubChatCompletionService
    {

        private readonly SemanticHubZhiPuConfig _config;
        private readonly ModelClient client;
        private readonly ILogger _log;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();
        public SemanticHubZhiPuChatCompletionService(SemanticHubZhiPuConfig config, ILoggerFactory? loggerFactory = null)
        {
            _config = config;
            client = new(_config.Secret, ModelType.ZhiPu, _config.Endpoint);
            _log = loggerFactory?.CreateLogger(typeof(SemanticHubZhiPuChatCompletionService));
        }

        (List<ChatMessage>, ChatParameters) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null, bool isStream = false)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
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

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            SemanticHubZhiPuChatResponseWrapper response = await client.ZhiPu.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<ChatMessageContent>();
            foreach (var item in response.Choices)
            {
                var res = new ChatMessageContent(AuthorRole.Assistant, item.Message.Content, metadata: metadata);
                result.Add(res);
            }
            return result;
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy);
            SemanticHubZhiPuChatResponseWrapper response = await client.ZhiPu.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<TextContent>();
            foreach (var item in response.Choices)
            {
                var res = new TextContent(item.Message.Content, metadata: metadata);
                result.Add(res);
            }
            return result;
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy, true);
            //返回流式聊天消息内容
            await foreach (var item in client.ZhiPu.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Choices[0].Message.Content, metadata: metadata);
                yield return result;
            }
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory, true);
            //返回流式聊天消息内容
            await foreach (var item in client.ZhiPu.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Choices[0].Message.Content, metadata: metadata);
                yield return result;
            }
        }

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubZhiPuChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(7)
            {
                { nameof(completions.Id), completions.Id },
                { nameof(completions.Model), completions.Model },
                { nameof(completions.Created), completions.Created },
                { nameof(completions.Choices), completions.Choices },
                { nameof(completions.RequestId), completions.RequestId },
                { nameof(completions.Usage), completions.Usage },
                { "Type", "ZhiPu" }
            };
        }
    }
}
