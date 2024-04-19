using Azure.AI.OpenAI;

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureImageToTextCompletion
{
    public class SemanticHubAzureOpenAIImageToTextCompletionService : ISemanticHubImageToTextService
    {
        private readonly SemanticHubAzureOpenAIConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubAzureOpenAIImageToTextCompletionService(SemanticHubAzureOpenAIConfig config)
        {
            _config = config;
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(ImageContent content, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            //(var histroyList, var chatParameters) = Init(executionSettings, chatHistroy);
            //SemanticHubAzureOpenAIChatResponseWrapper response = await client.AzureOpenAI.GetChatMessageContentsAsync(_config.DeploymentName, histroyList, chatParameters, cancellationToken);
            //List<TextContent> result = new List<TextContent>();
            //IReadOnlyDictionary<string, object?> metadata = GetResponseMetadata(response);
            //foreach (var item in response.Choices)
            //{
            //    var message = new TextContent(item.Message.Content, response.Model, metadata: metadata);
            //    result.Add(message);
            //}
            //return result;
            return new List<TextContent>();
        }
    }
}
