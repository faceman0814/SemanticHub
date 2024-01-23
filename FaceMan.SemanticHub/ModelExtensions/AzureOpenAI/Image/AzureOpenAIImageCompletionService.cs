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

        public async Task<List<string>> GetImageMessageContentsAsync(string prompt, ImageParameters parameters, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            AzureOpenAITextToImageService imageService = new AzureOpenAITextToImageService(
                config.DeploymentName,
                config.Endpoint,
                config.ApiKey,
                config.ModelId,
                apiVersion: config.ApiVersion);

            int width = 1024;
            int height = 1024;
            switch (parameters.ImageSize)
            {
                case SizeEnum.Size1280x720:
                    width = 1280;
                    height = 720;
                    break;
                case SizeEnum.Size720x1280:
                    height = 1280;
                    width = 720;
                    break;
            }
            var results = new List<string>();
            var url = await imageService.GenerateImageAsync(prompt, width, height);
            results.Add(url);
            return results;
        }
    }
}
