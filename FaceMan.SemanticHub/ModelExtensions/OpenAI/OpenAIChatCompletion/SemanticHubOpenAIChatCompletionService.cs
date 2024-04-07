using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System.Net.Http;

using ChatMessage = FaceMan.SemanticHub.Generation.ChatGeneration.ChatMessage;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat
{
    public class SemanticHubOpenAIChatCompletionService : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubOpenAIConfig _config;
        private readonly ModelClient client;
        private readonly ILogger _log;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubOpenAIChatCompletionService(SemanticHubOpenAIConfig config, ILoggerFactory? loggerFactory = null)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.OpenAI, config.Endpoint);
            _log = loggerFactory?.CreateLogger(typeof(SemanticHubOpenAIChatCompletionService));
        }

        (List<ChatMessage>, ChatParameters) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null, bool isStream = false)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = new ChatParameters()
            {
                TopP = settings != null ? (float)settings.TopP : (float)1.0,
                MaxTokens = settings != null ? settings.MaxTokens : 512,
                Temperature = settings != null ? (float)settings.Temperature : (float)1.0,
                PresencePenalty = settings != null ? (float)settings.PresencePenalty : (float)0.0,
                FrequencyPenalty = settings != null ? (float)settings.FrequencyPenalty : (float)0.0,
                Stream = isStream
            };
            var isOnlyOne = chatHistory.Count == 1;
            foreach (var item in chatHistory)
            {
                var history = new ChatMessage()
                {
                    Role = isOnlyOne ? "user" : item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            return (histroyList, chatParameters);
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            SemanticHubOpenAIChatResponseWrapper response = await client.OpenAI.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            var result = new List<ChatMessageContent>();
            foreach (var item in response.Choices)
            {
                var message = new ChatMessageContent(AuthorRole.Assistant, item.Message.Content, response.Model, metadata: metadata);
                result.Add(message);
            }
            return result;
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var chatHistroy = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, prompt)
            };
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy);
            SemanticHubOpenAIChatResponseWrapper response = await client.OpenAI.GetChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken);
            List<TextContent> result = new List<TextContent>();
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            foreach (var item in response.Choices)
            {
                var message = new TextContent(item.Message.Content, response.Model, metadata: metadata);
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
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistroy, true);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                foreach (var choice in item.Choices)
                {
                    var result = new StreamingTextContent(choice.Delta.Content, choice.Index, item.Model, metadata: metadata);
                    yield return result;
                }
            }
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory, true);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(_config.ModelName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                foreach (var choice in item.Choices)
                {
                    var result = new StreamingChatMessageContent(AuthorRole.Assistant, choice.Delta.Content, choice.Index, modelId: item.Model, metadata: metadata);
                    yield return result;
                }
            }
        }

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubOpenAIChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(6)
            {
                { nameof(completions.Id), completions.Id },
                { nameof(completions.Choices), completions.Choices },
                { nameof(completions.PromptFilterResults), completions.PromptFilterResults },
                { nameof(completions.Model), completions.Model },
                { nameof(completions.Usage), completions.Usage },
                { "Type", "OpenAI" },
            };
        }
    }
}
