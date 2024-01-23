using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;

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

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat
{
    public class AzureOpenAIChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly AzureOpenAIConfig config;
        private readonly ModelClient client;
        public AzureOpenAIChatCompletionService(string apiKey, string endPoint, string deploymentName, string apiVersion = null)
        {
            config = new AzureOpenAIConfig()
            {
                ApiKey = apiKey,
                Endpoint = endPoint,
                DeploymentName = deploymentName,
                ApiVersion = apiVersion
            };
            client = new(config.ApiKey, ModelType.AzureOpenAI, config.Endpoint);
        }
        (List<AzureOpenAIContextMessage>, ChatParameters) Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null)
        {
            var histroyList = new List<AzureOpenAIContextMessage>();
            ChatParameters chatParameters = new ChatParameters()
            {
                TopP = settings != null ? (float)settings.TopP : (float)1.0,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
                PresencePenalty = settings != null ? (float)settings.PresencePenalty : (float)0.0,
                FrequencyPenalty = settings != null ? (float)settings.FrequencyPenalty : (float)0.0,
            };
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
            return (histroyList, chatParameters);
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            AzureOpenAIResponseWrapper result = await client.AzureOpenAI.GetChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return (message, result.Usage);

        }

        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            AzureOpenAIResponseWrapper result = await client.AzureOpenAI.GetChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return message;
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            //返回流式聊天消息内容
            await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken))
            {
                yield return (item.Item1, item.Item2);
            }
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            //返回流式聊天消息内容
            await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(config.DeploymentName, histroyList, chatParameters, cancellationToken))
            {
                yield return item.Item1;
            }
        }
    }
}
