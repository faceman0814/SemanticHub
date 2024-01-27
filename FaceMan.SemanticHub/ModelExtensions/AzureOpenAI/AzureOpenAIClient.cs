using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;

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

        public async Task<AzureOpenAIChatResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<AzureOpenAIChatContextMessage> messages, ChatParameters parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(AzureOpenAIChatRequestWrapper.Create(messages, parameters)
                , options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<AzureOpenAIChatResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<AzureOpenAIChatContextMessage> messages,
        ChatParameters parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + $"openai/deployments/{model}/chat/completions?api-version={apiVersion}")
            {
                Content = JsonContent.Create(AzureOpenAIChatRequestWrapper.Create(messages, parameters),
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
                    var result = System.Text.Json.JsonSerializer.Deserialize<AzureOpenAIChatResponseWrapper>(data)!;
                    if (result.Choices.Any())
                    {
                        var content = string.Empty;
                        if (result.Choices?.First()?.Message != null)
                        {
                            content = result.Choices?.First()?.Message?.Content;
                        }
                        else
                        {
                            content = result.Choices?.First()?.Delta?.Content;
                        }
                        yield return (content, result.Usage);
                    }
                    continue;
                }
                else if (line.StartsWith("{\"error\":"))
                {
                    throw new Exception(line);
                }
            }
        }
    }
}
