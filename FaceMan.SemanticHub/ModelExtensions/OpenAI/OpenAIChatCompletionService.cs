using DocumentFormat.OpenXml.EMMA;

using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI
{
    public class OpenAIChatCompletionService
    {
        private readonly OpenAIConfig config;
        private readonly string _url;
        public OpenAIChatCompletionService(string apiKey, string modelId, string url = null)
        {
            config = new OpenAIConfig()
            {
                ApiKey = apiKey,
                ModelId = modelId,
            };
            _url = url;
        }

        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    MaxTokens = settings != null ? settings.MaxTokens : default,
                    Temperature = settings != null ? (float)settings.Temperature : default,
                    PresencePenalty = settings != null ? (float)settings.PresencePenalty : default,
                    FrequencyPenalty = settings != null ? (float)settings.FrequencyPenalty : default,
                };
            }
            ModelClient client = new(config.ApiKey, ModelType.OpenAI, _url);
            OpenAIResponseWrapper result = await client.OpenAI.GetChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken);
            var message = new ChatMessageContent(AuthorRole.Assistant, result.Choices.First().Message.Content);
            return message;
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    MaxTokens = settings != null ? settings.MaxTokens : default,
                    Temperature = settings != null ? (float)settings.Temperature : default,
                    PresencePenalty = settings != null ? (float)settings.PresencePenalty : default,
                    FrequencyPenalty = settings != null ? (float)settings.FrequencyPenalty : default,
                    Stream = true
                };
            }
            else
            {
                chatParameters = new ChatParameters()
                {
                    Stream = true
                };
            }
            ModelClient client = new(config.ApiKey, ModelType.OpenAI, _url);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(config.ModelId, histroyList, chatParameters, cancellationToken))
            {
                yield return item;
            }
        }
    }
}
