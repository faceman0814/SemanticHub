// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin
{
    public class WenXinClient
    {
        private readonly string baseUrl = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/";
        internal WenXinClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
        }
        internal ModelClient Parent { get; }

        public async Task<WenXinResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<ChatMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + model + $"?access_token={parameters.Token}")
            {
                Content = JsonContent.Create(WenXinRequestWrapper.Create(messages, parameters),
                options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<WenXinResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<ChatMessage> messages,
        ChatParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl + model + $"?access_token={parameters.Token}")
            {
                Content = JsonContent.Create(WenXinRequestWrapper.Create(messages, parameters),
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
                    var result = JsonSerializer.Deserialize<WenXinResponseWrapper>(data)!;
                    yield return (result.Result, result.Usage);
                }
                else if (line.StartsWith("{\"error\":"))
                {
                    throw new Exception(line);
                }
            }
        }
    }
}
