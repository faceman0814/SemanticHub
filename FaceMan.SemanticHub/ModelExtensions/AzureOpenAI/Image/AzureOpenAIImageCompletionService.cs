using Azure.AI.OpenAI;

using FaceMan.SemanticHub.ModelExtensions.ImageGeneration;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Image
{
    public class AzureOpenAIImageCompletionService : IModelExtensionsImageCompletionService
    {
        private readonly AzureOpenAIConfig config;
        public AzureOpenAIImageCompletionService(string apiKey, string endpoint, string? deploymentName, string? apiVersion = null)
        {
            config = new AzureOpenAIConfig()
            {
                Endpoint = endpoint,
                ApiVersion = apiVersion,
                ApiKey = apiKey,
                DeploymentName = deploymentName,
            };
        }

        public async Task<string> GetImageMessageContentsAsync(string description, int width, int height, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            AzureOpenAITextToImageService imageService = new AzureOpenAITextToImageService(
                config.DeploymentName,
                config.Endpoint,
                config.ApiKey,
                config.ModelId,
                apiVersion: config.ApiVersion);

            return await imageService.GenerateImageAsync(description, width, height);
        }
    }
}
