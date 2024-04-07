using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI
{
    public class SemanticHubAzureOpenAIClient
    {
        /// <summary>
        /// 基础请求地址
        /// </summary>
        private readonly string baseUrl;
        private readonly string apiVersion = "2023-07-01-preview";
        internal SemanticHubAzureOpenAIClient(ModelClient parent, string url, string _apiVersion = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
            apiVersion = _apiVersion ?? apiVersion;
        }
        internal ModelClient Parent { get; }

        public async Task<SemanticHubAzureOpenAIChatResponseWrapper> GetChatMessageContentsAsync(string model, List<SemanticHubAzureOpenAIChatContextMessage> messages, ChatParameters parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(SemanticHubAzureOpenAIChatRequestWrapper.Create(messages, parameters)
                , options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<SemanticHubAzureOpenAIChatResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<SemanticHubAzureOpenAIChatResponseWrapper> GetStreamingChatMessageContentsAsync(string model,
        List<SemanticHubAzureOpenAIChatContextMessage> messages,
        ChatParameters parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(SemanticHubAzureOpenAIChatRequestWrapper.Create(messages, parameters),
                options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };

            using HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }

            using StreamReader reader = new(await resp.Content.ReadAsStreamAsync(), Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                string line = await reader.ReadLineAsync();
                if (line != null)
                {
                    string data = line;
                    if (line.StartsWith("data:"))
                    {
                        data = line["data:".Length..];
                    }

                    if (data.Equals(" [DONE]") || string.IsNullOrEmpty(data))
                    {
                        continue;
                    }
                    var result = System.Text.Json.JsonSerializer.Deserialize<SemanticHubAzureOpenAIChatResponseWrapper>(data)!;
                    if (result.Choices.Any())
                    {
                        yield return result;
                    }
                    continue;
                }
                else if (line.StartsWith("{\"error\":"))
                {
                    throw new Exception(line);
                }
            }
        }

        public async Task<SemanticHubAzureOpenAITextEmbeddingResponseWrapper> GenerateEmbeddingsAsync(string model, string Input, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/embeddings?api-version={apiVersion}")
            {
                Content = JsonContent.Create(SemanticHubAzureOpenAITextEmbeddingRequestWrapper.Create(Input)
                , options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            var resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);

            return await ModelClient.ReadResponse<SemanticHubAzureOpenAITextEmbeddingResponseWrapper>(resp, cancellationToken);
        }
    }
}
