using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Text
{
    public class AzureOpenAITextCompletionService
    {
        private readonly AzureOpenAIConfig config;
        public AzureOpenAITextCompletionService(string apiKey, string endpoint, string? deploymentName, string? apiVersion = null)
        {
            config = new AzureOpenAIConfig()
            {
                Endpoint = endpoint,
                ApiVersion = apiVersion,
                ApiKey = apiKey,
                DeploymentName = deploymentName,
            };
        }

        public async Task<List<ReadOnlyMemory<float>>> GetTextMessageContentsAsync(string prompt, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            AzureOpenAITextEmbeddingGenerationService textService = new AzureOpenAITextEmbeddingGenerationService(
                config.DeploymentName,
                config.Endpoint,
                config.ApiKey,
                config.ModelId);
            List<string> se = new() { prompt };
            var result = await textService.GenerateEmbeddingsAsync(se);
            return (List<ReadOnlyMemory<float>>)result;
        }
    }
}
