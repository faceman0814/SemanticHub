// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
    public class XunFeiChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly XunFeiChatRequestWrapper xunFeiRequest;
        private readonly string _model;
        private readonly ModelClient client;
        public XunFeiChatCompletionService(string key, string secret, string appId, string model = null, string url = null)
        {
            xunFeiRequest = new XunFeiChatRequestWrapper()
            {
                key = key,
                Secret = secret,
                AppId = appId,
            };
            _model = model ?? "general";
            client = new(key, ModelType.XunFei, url);
        }

        /// <summary>
        /// 整合数据
        /// </summary>
        /// <param name="chatHistory">聊天历史</param>
        /// <param name="settings">模型参数</param>
        /// <returns></returns>
        private (XunFeiRequest, string) Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null)
        {
            XunFeiRequest request = new XunFeiRequest();
            request.header = new Header()
            {
                app_id = xunFeiRequest.AppId
            };
            var apiType = string.Empty;
            var modelName = string.Empty;
            switch (_model)
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
            foreach (var item in chatHistory)
            {
                var Content = new Content()
                {
                    content = item.Content,
                    role = item.Role.Label
                };
                request.payload.message.text.Add(Content);
            }

            return (request, apiType);
        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(chatHistory, settings);
            var result = await client.XunFei.GetChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Item1);
            return message;
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(chatHistory, settings);
            await foreach (var item in client.XunFei.GetStreamingChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken))
            {
                yield return item.Item1;
            }
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(chatHistory, settings);
            var result = await client.XunFei.GetChatMessageContentsAsync(request, xunFeiRequest, apiType, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Item1);
            return (message, result.Item2);
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var request, var apiType) = Init(chatHistory, settings);
            await foreach (var item in client.XunFei.GetStreamingChatMessageContentsAsync(request, xunFeiRequest, apiType,cancellationToken))
            {
                yield return (item.Item1, item.Item2);
            }
        }
    }
}
