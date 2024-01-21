using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI
{
    public class DefaultOpenAIChatCompletionService
    {
        private readonly OpenAIConfig config;
        private readonly string _url;
        public DefaultOpenAIChatCompletionService(string apiKey, string modelId, string url)
        {
            config.ApiKey = apiKey;
            config.ModelId = modelId;
            _url = url;
        }
        private OpenAIChatCompletionService InitOpenAIChat(OpenAIConfig config)
        {
            OpenAIChatCompletionService chatCompletionService = new(config.ModelId, config.ApiKey);
            return chatCompletionService;

        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            //初始化聊天服务
            var chatgpt = InitOpenAIChat(config);
            var result = await chatgpt.GetChatMessageContentsAsync(chatHistory, settings, kernel, cancellationToken);
            return result.First();
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            //初始化聊天服务
            var chatgpt = InitOpenAIChat(config);
            //返回流式聊天消息内容
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel, cancellationToken))
            {
                yield return item.Content;
            }
        }
    }
}
