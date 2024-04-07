using Azure.AI.OpenAI;

using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions
{
#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    public interface ISemanticHubTextToImageService : ITextToImageService
    {
        Task<List<ImageContext>> GenerateImageAsync(
        ImageGenerationOptions imageParameters,
        Kernel kernel = null,
        CancellationToken cancellationToken = default);
    }
}
