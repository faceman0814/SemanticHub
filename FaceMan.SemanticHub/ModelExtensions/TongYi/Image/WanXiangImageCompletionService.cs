using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using DocumentFormat.OpenXml.Spreadsheet;

using FaceMan.SemanticHub.ModelExtensions.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;

using Microsoft.SemanticKernel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Microsoft.Graph.Constants;

using Task = System.Threading.Tasks.Task;

namespace FaceMan.SemanticHub.ModelExtensions.TongYi.Image
{
    public class WanXiangImageCompletionService : IModelExtensionsImageCompletionService
    {
        private readonly string _model;
        private readonly ModelClient client;
        public WanXiangImageCompletionService(string key, string model, string url = null)
        {
            _model = model;
            client = new(key, ModelType.TongYi, url);
        }

        public async Task<List<string>> GetImageMessageContentsAsync(string prompt, ImageParameters parameters, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var results = new List<string>();
            WanXiangResponseWrapper result = await client.TongYi.GetImageMessageContentsAsync(_model, prompt, parameters, cancellationToken);
            while (true)
            {
                ImageTaskStatusResponseWrapper resp = await client.TongYi.QueryTaskStatus(result.TaskId, cancellationToken);
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
