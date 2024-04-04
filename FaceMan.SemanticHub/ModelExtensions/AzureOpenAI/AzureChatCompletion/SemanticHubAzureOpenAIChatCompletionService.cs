using Azure.AI.OpenAI;

using DocumentFormat.OpenXml.Wordprocessing;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Org.BouncyCastle.Asn1.Ocsp;

using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion
{
    public class SemanticHubAzureOpenAIChatCompletionService : ISemanticHubChatCompletionService
    {
        private readonly SemanticHubAzureOpenAIConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubAzureOpenAIChatCompletionService(SemanticHubAzureOpenAIConfig config)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.AzureOpenAI, config.Endpoint, apiVersion: config.ApiVersion);
        }
        (List<SemanticHubAzureOpenAIChatContextMessage>, ChatParameters) Init(PromptExecutionSettings executionSettings, ChatHistory chatHistory = null)
        {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            var histroyList = new List<SemanticHubAzureOpenAIChatContextMessage>();
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
                var history = new SemanticHubAzureOpenAIChatContextMessage()
                {
                    Role = item.Role.Label,
                    Content = new List<Content>()
                    {
                        new Content()
                        {
                            Type = "text",
                            Text = item.Content
                        }
                    },
                };
                histroyList.Add(history);
            }
            return (histroyList, chatParameters);
        }


        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings);
            SemanticHubAzureOpenAIChatResponseWrapper response = await client.AzureOpenAI.GetChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken);
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
            await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingTextContent(item.Choices[0].Message.Content, item.Choices[0].Index, item.Model, metadata: metadata);
                yield return result;
            }
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            SemanticHubAzureOpenAIChatResponseWrapper response = await client.AzureOpenAI.GetChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken);
            IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            List<ChatMessageContent> result = new List<ChatMessageContent>();
            foreach (var item in response.Choices)
            {
                var message = new ChatMessageContent(AuthorRole.Assistant, item.Message.Content, metadata: metadata);
                result.Add(message);
            }
            return result;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
            //返回流式聊天消息内容
            await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken))
            {
                IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(item);
                var result = new StreamingChatMessageContent(AuthorRole.Assistant, item.Choices[0].Message.Content, choiceIndex: item.Choices[0].Index, modelId: item.Model, metadata: metadata);
                yield return result;
            }
        }

        //public async IAsyncEnumerable<(StreamingChatMessageContent, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        //{
        //    (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
        //    //返回流式聊天消息内容
        //    await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken))
        //    {
        //        var textContent = new StreamingChatMessageContent(AuthorRole.Assistant, item.Item1);
        //        yield return (textContent, item.Item2);
        //    }
        //}

        //public async Task<(IReadOnlyList<ChatMessageContent>, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        //{
        //    (var histroyList, var chatParameters) = Init(executionSettings, chatHistory);
        //    AzureOpenAIChatResponseWrapper result = await client.AzureOpenAI.GetChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken);
        //    List<ChatMessageContent> textContents = new List<ChatMessageContent>();
        //    foreach (var item in result.Choices)
        //    {
        //        var message = new ChatMessageContent(AuthorRole.Assistant, item.Message.Content);
        //        textContents.Add(message);
        //    }
        //    return (textContents, result.Usage);
        //}

        //public async IAsyncEnumerable<(StreamingTextContent, Usage)> GetStreamingTextContentsByTokenAsync(string prompt, OpenAIPromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        //{
        //    (var histroyList, var chatParameters) = Init(executionSettings);
        //    //返回流式聊天消息内容
        //    await foreach (var item in client.AzureOpenAI.GetStreamingChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken))
        //    {
        //        var textContent = new StreamingTextContent(item.Item1);
        //        yield return (textContent, item.Item2);
        //    }
        //}

        //public async Task<(IReadOnlyList<TextContent>, Usage)> GetTextContentsByTokenAsync(string prompt, OpenAIPromptExecutionSettings executionSettings, Kernel kernel, CancellationToken cancellationToken)
        //{
        //    (var histroyList, var chatParameters) = Init(executionSettings);
        //    AzureOpenAIChatResponseWrapper result = await client.AzureOpenAI.GetChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken);
        //    List<TextContent> textContents = new List<TextContent>();
        //    foreach (var item in result.Choices)
        //    {
        //        var message = new TextContent(item.Message.Content);
        //        textContents.Add(message);
        //    }
        //    return (textContents, result.Usage);
        //}

        private Dictionary<string, object?> GetResponseMetadata(SemanticHubAzureOpenAIChatResponseWrapper completions)
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
