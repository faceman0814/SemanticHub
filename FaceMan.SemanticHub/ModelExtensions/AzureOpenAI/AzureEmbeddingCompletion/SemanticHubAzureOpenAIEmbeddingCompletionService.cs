using Microsoft.SemanticKernel;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion
{
    public class SemanticHubAzureOpenAIEmbeddingCompletionService : ISemanticHubTextEmbeddingGenerationService
    {
        private readonly SemanticHubAzureOpenAIConfig _config;
        private readonly ModelClient client;
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public SemanticHubAzureOpenAIEmbeddingCompletionService(SemanticHubAzureOpenAIConfig config)
        {
            _config = config;
            client = new(config.ApiKey, ModelType.AzureOpenAI, config.Endpoint, apiVersion: config.ApiVersion);
        }


        public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var result = await client.AzureOpenAI.GenerateEmbeddingsAsync(_config.DeploymentName, data.First());
            var list = result.Data[0].Embedding.ToList();
            IList<ReadOnlyMemory<float>> readOnlyMemoryList = list.Select(f => new ReadOnlyMemory<float>(new float[] { f })).ToList();
            return readOnlyMemoryList;
        }

        public async Task<(List<float>, TextEmbeddingUsage)> GenerateEmbeddingsByUsageAsync(IList<string> data, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var result = await client.AzureOpenAI.GenerateEmbeddingsAsync(_config.DeploymentName, data.First());
            var list = result.Data[0].Embedding.ToList();
            IList<ReadOnlyMemory<float>> readOnlyMemoryList = list.Select(f => new ReadOnlyMemory<float>(new float[] { f })).ToList();
            return (list, result.Usage);
        }
    }
}
