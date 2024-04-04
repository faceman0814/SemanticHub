using DocumentFormat.OpenXml.Wordprocessing;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System.Collections.Generic;

using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat
{
    public class SemanticHubOpenAIChatCompletionService : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubOpenAIConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubOpenAIChatCompletionService(SemanticHubOpenAIConfig config)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.OpenAI, config.BaseUrl);
        }
        (List<ChatMessage>, ChatParameters) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null)
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

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            SemanticHubOpenAIChatResponseWrapper response = await client.OpenAI.GetChatMessageContentsAsync(_config.ModelId, histroyList, chatParameters, cancellationToken);
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
            (var histroyList, var chatParameters) = Init(executionSettings);
            SemanticHubOpenAIChatResponseWrapper response = await client.OpenAI.GetChatMessageContentsAsync(_config.ModelId, histroyList, chatParameters, cancellationToken);
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
            (var histroyList, var chatParameters) = Init(executionSettings);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(_config.ModelId, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Choices[0].Message.Content, item.Choices[0].Index, item.Model, metadata: metadata);
                yield return result;
            }
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            //返回流式聊天消息内容
            await foreach (var item in client.OpenAI.GetStreamingChatMessageContentsAsync(_config.ModelId, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Choices[0].Message.Content, choiceIndex: item.Choices[0].Index, modelId: item.Model, metadata: metadata);
                yield return result;
            }
        }

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubOpenAIChatResponseWrapper completions)
        {
            return new Dictionary<string, object?>(5)
            {
                { nameof(completions.Id), completions.Id },
                { nameof(completions.Choices), completions.Choices },
                { nameof(completions.PromptFilterResults), completions.PromptFilterResults },
                { nameof(completions.Model), completions.Model },
                { nameof(completions.Usage), completions.Usage },
            };
        }
    }
}
