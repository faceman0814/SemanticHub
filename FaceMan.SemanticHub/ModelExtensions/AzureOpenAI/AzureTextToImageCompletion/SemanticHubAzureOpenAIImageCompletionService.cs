using Azure;
using Azure.AI.OpenAI;

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;

using FaceMan.SemanticHub.Generation.ImageGeneration;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion
{
    public class SemanticHubAzureOpenAIImageCompletionService : ISemanticHubTextToImageService
    {
        private readonly SemanticHubAzureOpenAIConfig _config;
        private readonly OpenAIClient _client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubAzureOpenAIImageCompletionService(SemanticHubAzureOpenAIConfig config)
        {
            _config = config;
        }

        public async Task<string> GenerateImageAsync(string description, int width, int height, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            AzureOpenAITextToImageService imageService = new(
                   _config.DeploymentName,
                   _config.Endpoint,
                   _config.ApiKey,
                   null,
             apiVersion: _config.ApiVersion);
            return await imageService.GenerateImageAsync(description, width, height);
        }

        public async Task<List<ImageContext>> GenerateImageAsync(
        ImageGenerationOptions imageParameters,
        Kernel kernel = null,
        CancellationToken cancellationToken = default)
        {
            OpenAIClient client = new(new Uri(_config.Endpoint), new AzureKeyCredential(_config.ApiKey));
            var imageGenerations = await client.GetImageGenerationsAsync(imageParameters, cancellationToken);
            var result = new List<ImageContext>();
            foreach (var item in imageGenerations.Value.Data)
            {
                var imageUri = item.Url.ToString();
                var revisedPrompt = item.RevisedPrompt;
                var base64Data = item.Base64Data;
                byte[] file = default;
                if (string.IsNullOrEmpty(base64Data))
                {
                    file = await DownloadImageAsync(imageUri);

                }
                else
                {
                    file = Convert.FromBase64String(base64Data);
                }
                var res = new ImageContext(imageUri, revisedPrompt, base64Data, file);
                result.Add(res);
            }

            return result;
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception("无法下载图片: " + response.ReasonPhrase);
                }
            }
        }
    }

    public class ImageContext
    {
        public ImageContext(string url, string revisedPrompt = null, string base64Data = null, byte[] file = null)
        {
            this.ImageUrL = url;
            this.RevisedPrompt = revisedPrompt;
            this.Base64Data = base64Data;
            this.File = file;
        }

        public string ImageUrL { get; set; }
        public string RevisedPrompt { get; set; }
        public string Base64Data { get; set; }
        public byte[] File { get; set; }


    }
}
