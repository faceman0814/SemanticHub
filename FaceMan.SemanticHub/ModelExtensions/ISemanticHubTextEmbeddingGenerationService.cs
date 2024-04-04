using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions
{
    public interface ISemanticHubTextEmbeddingGenerationService : ITextEmbeddingGenerationService
    {
        Task<(List<float>, TextEmbeddingUsage)> GenerateEmbeddingsByUsageAsync(IList<string> data, Kernel kernel = null, CancellationToken cancellationToken = default);
    }
}
