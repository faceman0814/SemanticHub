using FaceMan.SemanticHub.Generation.ImageGeneration;

using Microsoft.SemanticKernel;

using Task = System.Threading.Tasks.Task;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    public class TongYiImageCompletionService : IModelExtensionsImageCompletionService
    {
        private readonly string _model;
        private readonly ModelClient client;
        public TongYiImageCompletionService(string key, string model, string url = null)
        {
            _model = model;
            client = new(key, ModelType.TongYi, url);
        }

        public async Task<List<string>> GetImageMessageContentsAsync(string prompt, ImageParameters parameters, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var results = new List<string>();
            TongYiImageResponseWrapper result = await client.TongYi.GetImageMessageContentsAsync(_model, prompt, parameters, cancellationToken);
            while (true)
            {
                TongYiImageTaskStatusResponseWrapper resp = await client.TongYi.QueryTaskStatus(result.TaskId, cancellationToken);
                if (resp.TaskStatus == ImageTaskStatusEnum.Succeeded)
                {
                    SuccessTaskResponseWrapper success = resp.AsSuccess();
                    using HttpClient client = new();
                    for (int i = 0; i < success.Results.Count; i++)
                    {
                        string url = success.Results[i].Url!;
                        results.Add(url);
                        //using HttpResponseMessage imgResp = await client.GetAsync(url);
                        //using Stream imgStream = await imgResp.Content.ReadAsStreamAsync();
                        //using FileStream fs = new($"{nameof(StableDiffusionText2ImageTest)}-{i}.png", FileMode.Create);
                        //await imgStream.CopyToAsync(fs);
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

    }
}
