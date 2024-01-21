using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Microsoft.SemanticKernel.Connectors.OpenAI;

using Newtonsoft.Json;

using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.AzureOpenAI
{
    public class AzureOpenAIClient
    {
        /// <summary>
        /// 基础请求地址
        /// </summary>
        private readonly string baseUrl = "https://faceman.openai.azure.com/";
        private readonly string apiVersion = "2023-07-01-preview";
        internal AzureOpenAIClient(ModelClient parent, string url = null, string _apiVersion = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
            apiVersion = _apiVersion ?? apiVersion;
        }
        internal ModelClient Parent { get; }

        public async Task<AzureOpenAIResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<AzureOpenAIContextMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(AzureOpenAIRequestWrapper.Create(messages, parameters)
                , options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<AzureOpenAIResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<AzureOpenAIContextMessage> messages,
        ChatParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(AzureOpenAIRequestWrapper.Create(messages, parameters),
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

                string? line = await reader.ReadLineAsync();
                if (line != null && line.StartsWith("data:"))
                {
                    string data = line["data:".Length..];
                    if (data.Equals(" [DONE]"))
                    {
                        continue;
                    }
                    var result = System.Text.Json.JsonSerializer.Deserialize<AzureOpenAIResponseWrapper>(data)!;
                    if (result.Choices.Any())
                    {
                        yield return result.Choices?.First()?.Delta?.Content;
                    }
                    yield return "";
                }
                else if (line.StartsWith("{\"error\":"))
                {
                    throw new Exception(line);
                }
            }
        }
    }
}
