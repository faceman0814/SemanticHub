using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.OpenAI
{
    public class SemanticHubOpenAIClient
    {
        /// <summary>
        /// 基础请求地址
        /// </summary>
        private readonly string baseUrl = "https://api.openai.com/v1";
        internal SemanticHubOpenAIClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
        }
        internal ModelClient Parent { get; }

        public async Task<SemanticHubOpenAIChatResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<ChatMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + "/chat/completions")
            {
                Content = JsonContent.Create(SemanticHubOpenAIChatRequestWrapper.Create(model, messages, parameters)
                , options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<SemanticHubOpenAIChatResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<SemanticHubOpenAIChatResponseWrapper> GetStreamingChatMessageContentsAsync(string model,
       IReadOnlyList<ChatMessage> messages,
       ChatParameters? parameters = null,
       [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + "/chat/completions")
            {
                Content = JsonContent.Create(SemanticHubOpenAIChatRequestWrapper.Create(model, messages, parameters),
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
                    var result = System.Text.Json.JsonSerializer.Deserialize<SemanticHubOpenAIChatResponseWrapper>(data)!;
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
    }
}
