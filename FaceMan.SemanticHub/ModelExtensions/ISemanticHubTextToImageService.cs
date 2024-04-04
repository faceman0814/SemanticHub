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
    public interface ISemanticHubTextToImageService : ITextToImageService
    {
        Task<List<ImageContext>> GenerateImageAsync(string prompt,
        ImageGenerationOptions imageParameters,
        Kernel kernel = null,
        CancellationToken cancellationToken = default);
    }
}
