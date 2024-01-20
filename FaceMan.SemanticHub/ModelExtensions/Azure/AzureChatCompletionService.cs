using DocumentFormat.OpenXml.EMMA;

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
        public AzureChatCompletionService(string apiKey, string endPoint, string deploymentName, string modelId = null)
        {
            config.ApiKey = apiKey;
            config.Endpoint = endPoint;
            config.DeploymentName = deploymentName;
            config.ModelId = modelId;
        }
        private AzureOpenAIChatCompletionService InitAzureOpenAIChat(AzureOpenAIConfig config)
        {
            AzureOpenAIChatCompletionService chatCompletionService = new(
                deploymentName: config.DeploymentName,
                endpoint: config.Endpoint,
                apiKey: config.ApiKey,
                modelId: config.ChatModelId);
            return chatCompletionService;

        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            //初始化聊天服务
            var chatgpt = InitAzureOpenAIChat(config);
            return await chatgpt.GetChatMessageContentAsync(chatHistory, settings, kernel, cancellationToken);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            //初始化聊天服务
            var chatgpt = InitAzureOpenAIChat(config);
            //返回流式聊天消息内容
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel, cancellationToken))
            {
                yield return item.Content;
            }
        }
    }
}
