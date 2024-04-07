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
#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    public interface ISemanticHubTextEmbeddingGenerationService : ITextEmbeddingGenerationService
    {
        Task<(List<float>, TextEmbeddingUsage)> GenerateEmbeddingsByUsageAsync(IList<string> data, Kernel kernel = null, CancellationToken cancellationToken = default);
    }
}
