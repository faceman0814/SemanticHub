// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
    public class QianWenChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _url;
        public QianWenChatCompletionService(string key, string model, string url = null)
        {
            _apiKey = key;
            _model = model;
            _url = url;
        }

        /// <summary>
        /// 对话
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <param name="settings"></param>
        /// <param name="kernel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters  = new ChatParameters()
            {
                TopP = settings != null&&settings.TopP!=1.0 ? (float)settings.TopP : (float)0.75,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
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

            ModelClient client = new(_apiKey, ModelType.QianWen, _url);
            QianWenResponseWrapper result = await client.QianWen.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Output.Text);
            return message;
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters  = new ChatParameters()
            {
                TopP = settings != null && settings.TopP != 1.0 ? (float)settings.TopP : (float)0.75,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
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

            ModelClient client = new(_apiKey, ModelType.QianWen, _url);
            QianWenResponseWrapper result = await client.QianWen.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Output.Text);
            var usage = new Usage()
            {
                PromptTokens = result.Usage.InputTokens,
                TotalTokens = result.Usage.TotalTokens,
                CompletionTokens = result.Usage.OutputTokens
            };
            return (message, usage);
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters  = new ChatParameters()
            {
                TopP = settings != null && settings.TopP != 1.0 ? (float)settings.TopP : (float)0.75,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
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
            ModelClient client = new(_apiKey, ModelType.QianWen, _url);

            await foreach (var item in client.QianWen.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                var usage = new Usage()
                {
                    PromptTokens = item.Item2.InputTokens,
                    TotalTokens = item.Item2.TotalTokens,
                    CompletionTokens = item.Item2.OutputTokens
                };
                yield return (item.Item1, usage);
            }
        }

        /// <summary>
        /// 流式对话
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <param name="settings"></param>
        /// <param name="kernel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters  = new ChatParameters()
            {
                TopP = settings != null && settings.TopP != 1.0 ? (float)settings.TopP : (float)0.75,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
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
           
            ModelClient client = new(_apiKey, ModelType.QianWen, _url);

            await foreach (var item in client.QianWen.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return item.Item1;
            }
        }

    }

}
