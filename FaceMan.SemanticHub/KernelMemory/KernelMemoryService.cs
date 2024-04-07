using FaceMan.SemanticHub.HttpClients;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;

namespace FaceMan.SemanticHub.KernelMemory
{
    [ServiceDescription(typeof(IKernelMemoryService), ServiceLifetime.Singleton)]
    public class KernelMemoryService : IKernelMemoryService
    {
        private MemoryServerless _memory;
        public MemoryServerless GetMemoryByKMS(SearchClientConfig searchClientConfig = null)
        {
            //if (_memory.IsNull())
            {

                //http代理
                var chatHttpClient = OpenAIHttpClientHandlerUtil.GetHttpClient("https://yoyochatx.openai.azure.com/");
                var embeddingHttpClient = OpenAIHttpClientHandlerUtil.GetHttpClient("https://yoyochatx.openai.azure.com/");

                //搜索配置
                if (searchClientConfig == null)
                {
                    searchClientConfig = new SearchClientConfig
                    {
                        MaxAskPromptSize = 2048,
                        MaxMatchesCount = 3,
                        AnswerTokens = 1000,
                        EmptyAnswer = "知识库未搜索到相关内容"
                    };
                }
                var memoryBuild = new KernelMemoryBuilder()
                .WithSearchClientConfig(searchClientConfig)
                .WithCustomTextPartitioningOptions(new TextPartitioningOptions())
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig()
                {
                    APIKey = "28df2a037e7c432a85f7892d280b99c9",
                    Deployment = "text-embedding-ada-002",
                    Endpoint = "https://yoyochatx.openai.azure.com/",
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIType =AzureOpenAIConfig.APITypes.TextCompletion,
                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig()
                {
                    APIKey = "28df2a037e7c432a85f7892d280b99c9",
                    Deployment = "text-embedding-ada-002",
                    Endpoint = "https://yoyochatx.openai.azure.com/",
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                }).WithSimpleFileStorage(new SimpleFileStorageConfig()
                {
                    StorageType = FileSystemTypes.Volatile
                }); ;

                _memory = memoryBuild.Build<MemoryServerless>();
                return _memory;
            }
            //else {
            //    return _memory;
            //}

        }
    }
}
