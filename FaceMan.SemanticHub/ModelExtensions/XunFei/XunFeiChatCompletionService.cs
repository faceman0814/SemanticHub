// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;

using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
    public class XunFeiChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly XunFeiRequestWrapper xunFeiRequest;
        private readonly string _model;
        private readonly string _url;
        public XunFeiChatCompletionService(string key, string secret, string appId, string model, string url = null)
        {
            xunFeiRequest = new XunFeiRequestWrapper()
            {
                key = key,
                Secret = secret,
                AppId = appId,
            };
            _model = model;
            _url = url;
        }

        /// <summary>
        /// 整合数据
        /// </summary>
        /// <param name="chatHistory">聊天历史</param>
        /// <param name="settings">模型参数</param>
        /// <returns></returns>
        private XunFeiRequest Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null)
        {
            XunFeiRequest request = new XunFeiRequest();
            request.header = new Header()
            {
                app_id = xunFeiRequest.AppId
            };
            request.parameter = new Parameter()
            {
                chat = new Chat()
                {
                    domain = _model,//模型领域，默认为星火通用大模型
                    temperature = settings != null ? settings.Temperature : 0.7, //温度采样阈值，用于控制生成内容的随机性和多样性，值越大多样性越高；范围（0，1）
                    max_tokens = settings != null && settings.MaxTokens != null ? settings.MaxTokens.Value : 1024,//生成内容的最大长度，范围（0，4096）
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
            return request;
        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var request = Init(chatHistory, settings);
            ModelClient client = new(xunFeiRequest.key, ModelType.XunFei, _url);
            var result = await client.XunFei.GetChatMessageContentsAsync(request, xunFeiRequest, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result);
            return message;
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var request = Init(chatHistory, settings);
            ModelClient client = new(xunFeiRequest.key, ModelType.XunFei, _url);
            await foreach (string item in client.XunFei.GetStreamingChatMessageContentsAsync(request, xunFeiRequest, cancellationToken))
            {
                yield return item;
            }
        }
    }
}
