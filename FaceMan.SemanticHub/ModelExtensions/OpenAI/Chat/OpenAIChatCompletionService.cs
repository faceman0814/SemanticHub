using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat
{
    public class OpenAIChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly OpenAIConfig config;
        private readonly ModelClient client;
        public OpenAIChatCompletionService(string apiKey, string modelId, string url = null)
        {
            config = new OpenAIConfig()
            {
                ApiKey = apiKey,
                ModelId = modelId,
            };
            client = new(config.ApiKey, ModelType.OpenAI, url);
        }
        (List<ChatMessage>, ChatParameters) Init(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null)
        {
            var histroyList = new List<ChatMessage>();
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
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            return (histroyList, chatParameters);
        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            OpenAIChatResponseWrapper result = await client.OpenAI.GetChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return message;
        }

        public async Task<(ChatMessageContent, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            OpenAIChatResponseWrapper result = await client.OpenAI.GetChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return (message, result.Usage);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken))
            {
                yield return item.Item1;
            }
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(chatHistory, settings);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken))
            {
                yield return (item.Item1, item.Item2);
            }
        }
    }
}
