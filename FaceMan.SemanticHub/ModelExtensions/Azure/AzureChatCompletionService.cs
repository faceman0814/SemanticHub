﻿using DocumentFormat.OpenXml.EMMA;

using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.Azure
{
    public class AzureChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly AzureOpenAIConfig config;
        public AzureChatCompletionService(string apiKey, string endPoint, string deploymentName, string apiVersion = null)
        {
            config = new AzureOpenAIConfig()
            {
                ApiKey = apiKey,
                Endpoint = endPoint,
                DeploymentName = deploymentName,
                ApiVersion = apiVersion
            };
        }

        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<AzureOpenAIContextMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                var history = new AzureOpenAIContextMessage()
                {
                    Role = item.Role.Label,
                    Content = new List<Content>()
                    {
                        new Content()
                        {
                            Type="text",
                            Text=item.Content
                        }
                    },
                };
                histroyList.Add(history);
            }
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    MaxTokens = settings != null ? settings.MaxTokens : default,
                    Temperature = settings != null ? (float)settings.Temperature : default
                };
            }
            ModelClient client = new(config.ApiKey, ModelType.AzureOpenAI, config.Endpoint);
            AzureOpenAIResponseWrapper result = await client.AzureOpenAI.GetChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return message;
        }



        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<AzureOpenAIContextMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                var history = new AzureOpenAIContextMessage()
                {
                    Role = item.Role.Label,
                    Content = new List<Content>()
                    {
                        new Content()
                        {
                            Type="text",
                            Text=item.Content
                        }
                    },
                };
                histroyList.Add(history);
            }
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    MaxTokens = settings != null ? settings.MaxTokens : default,
                    Temperature = settings != null ? (float)settings.Temperature : default
                };
            }
            else
            {
                chatParameters = new ChatParameters()
                {
                    Stream = true
                };
            }
            ModelClient client = new(config.ApiKey, ModelType.AzureOpenAI, config.Endpoint);
            //返回流式聊天消息内容
            await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken))
            {
                yield return item;
            }
        }
    }
}
