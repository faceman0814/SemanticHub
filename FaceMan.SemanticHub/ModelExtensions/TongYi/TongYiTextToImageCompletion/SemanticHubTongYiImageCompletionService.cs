using Azure.AI.OpenAI;

using DocumentFormat.OpenXml.Spreadsheet;

using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.SemanticKernel;

using static Org.BouncyCastle.Math.EC.ECCurve;

using Task = System.Threading.Tasks.Task;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    public class SemanticHubTongYiImageCompletionService : ISemanticHubTextToImageService
    {
        private readonly SemanticHubTongYiConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();
        public SemanticHubTongYiImageCompletionService(SemanticHubTongYiConfig config)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.TongYi, config.Endpoint);
        }

        public async Task<List<ImageContext>> GenerateImageAsync(ImageGenerationOptions imageParameters, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var results = new List<ImageContext>();
            SemanticHubTongYiImageResponseWrapper response = await client.TongYi.GetImageMessageContentsAsync(imageParameters.DeploymentName, imageParameters.Prompt, _config.ImageParameters, cancellationToken);
            while (true)
            {
                SemanticHubTongYiImageTaskStatusResponseWrapper resp = await client.TongYi.QueryTaskStatus(response.TaskId, cancellationToken);
                if (resp.TaskStatus == ImageTaskStatusEnum.Succeeded)
                {
                    SuccessTaskResponseWrapper success = resp.AsSuccess();
                    using HttpClient client = new();
                    foreach (var item in success.Results)
                    {
                        string url = item.Url!;
                        byte[] file = await DownloadImageAsync(url);
                        var res = new ImageContext(url, file: file);
                        results.Add(res);
                    }
                    break;
                }
                else if (resp.TaskStatus == ImageTaskStatusEnum.Failed)
                {
                    FailedTaskResponseWrapper failed = resp.AsFailed();
                    throw new Exception($"Failed! reason: {failed.Code} {failed.Message}");
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
            return results;
        }

        public async Task<string> GenerateImageAsync(string description, int width, int height, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            SemanticHubTongYiImageResponseWrapper response = await client.TongYi.GetImageMessageContentsAsync(_config.ModelName, description, _config.ImageParameters, cancellationToken);
            while (true)
            {
                SemanticHubTongYiImageTaskStatusResponseWrapper resp = await client.TongYi.QueryTaskStatus(response.TaskId, cancellationToken);
                if (resp.TaskStatus == ImageTaskStatusEnum.Succeeded)
                {
                    SuccessTaskResponseWrapper success = resp.AsSuccess();
                    using HttpClient client = new();
                    foreach (var item in success.Results)
                    {
                        string url = item.Url!;
                        return url;
                    }
                    break;
                }
                else if (resp.TaskStatus == ImageTaskStatusEnum.Failed)
                {
                    FailedTaskResponseWrapper failed = resp.AsFailed();
                    throw new Exception($"Failed! reason: {failed.Code} {failed.Message}");
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
            return null;
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
}
