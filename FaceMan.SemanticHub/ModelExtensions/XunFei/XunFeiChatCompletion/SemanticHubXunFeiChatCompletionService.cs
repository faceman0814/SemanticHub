// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.Wordprocessing;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
    public class SemanticHubXunFeiChatCompletionService
    : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubXunFeiChatRequestWrapper xunFeiRequest;
        private readonly SemanticHubXunFeiConfig _config;
        private readonly ModelClient client;
        private readonly ILogger _log;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();
        public SemanticHubXunFeiChatCompletionService(SemanticHubXunFeiConfig config, ILoggerFactory? loggerFactory = null)
        {
            xunFeiRequest = new SemanticHubXunFeiChatRequestWrapper()
            {
                key = config.ApiKey,
                Secret = config.Secret,
                AppId = config.AppId,
            };
            _config = config;
            client = new(config.ApiKey, ModelType.XunFei, config.Endpoint);
            _log = loggerFactory?.CreateLogger(typeof(SemanticHubAzureOpenAIChatCompletionService));
        }

        /// <summary>
        /// 整合数据
        /// </summary>
        /// <param name="chatHistory">聊天历史</param>
        /// <param name="settings">模型参数</param>
        /// <returns></returns>
        private (XunFeiRequest, string) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
            XunFeiRequest request = new XunFeiRequest();
            request.header = new Header()
            {
                app_id = _config.AppId
            };
            var apiType = string.Empty;
            var modelName = string.Empty;
            switch (_config.ModelName)
            {
                case "Spark V3.5":
                    apiType = "v3.5";
                    modelName = "generalv3.5";
                    break;
                case "Spark V3.0":
                    apiType = "v3.1";
                    modelName = "generalv3";
                    break;
                case "Spark V2.0":
                    apiType = "v2.1";
                    modelName = "generalv2";
                    break;
                case "Spark V1.5":
                    apiType = "v1.1";
                    modelName = "general";
                    break;
            }
            request.parameter = new Parameter()
            {
                chat = new Chat()
                {
                    domain = modelName,//模型领域，默认为星火通用大模型
                    temperature = settings != null ? settings.Temperature : 0.5, //温度采样阈值，用于控制生成内容的随机性和多样性，值越大多样性越高；范围（0，1）
                    max_tokens = settings != null && settings.MaxTokens != null ? settings.MaxTokens.Value : 512,//生成内容的最大长度，范围（0，4096）
                    top_k = 1
                }
            };
            request.payload = new Payload()
            {
                message = new Message()
                {
                    text = new List<Content>()
                }
            };
            if (chatHistory != null)
            {
                foreach (var item in chatHistory)
                {
                    var Content = new Content()
                    {
                        content = item.Content,
                        role = item.Role.Label
                    };
                    request.payload.message.text.Add(Content);
                }
            }
            return (request, apiType);
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var request, var apiType) = Init(executionSettings, chatHistroy);
            var context = new Content()
            {
                content = prompt,
                role = "user"
            };
            request.payload.message.text.Add(context);
            var response = await client.XunFei.GetChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken);
            List<TextContent> result = new List<TextContent>();
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            foreach (var item in response.Payload.Choices.Text)
            {
                var message = new TextContent(item.Content, metadata: metadata);
                result.Add(message);
            }
            return result;
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var request, var apiType) = Init(executionSettings, chatHistroy);
            //返回流式聊天消息内容
            await foreach (var item in client.XunFei.GetStreamingChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Payload.Choices.Text[0].Content, metadata: metadata);
                yield return result;
            }
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(executionSettings, chatHistory);
            var response = await client.XunFei.GetChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken);
            List<ChatMessageContent> result = new List<ChatMessageContent>();
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            foreach (var item in response.Payload.Choices.Text)
            {
                var message = new ChatMessageContent(AuthorRole.Assistant, item.Content, metadata: metadata);
                result.Add(message);
            }
            return result;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(executionSettings, chatHistory);
            //返回流式聊天消息内容
            await foreach (var item in client.XunFei.GetStreamingChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Payload.Choices.Text[0].Content, metadata: metadata);
                yield return result;
            }
        }


        private Dictionary<string, object?> GetResponseMetadata(SemanticHubXunFeiChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(3)
            {
                { nameof(completions.Header), completions.Header },
                { nameof(completions.Payload), completions.Payload },
                { "Type", "XunFei" },
            };
        }
    }
}
