// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu
{
    public class ZhiPuClient
    {
        private readonly string baseUrl = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
        internal ZhiPuClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
        }
        internal ModelClient Parent { get; }

        public async Task<ZhiPuChatResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<ChatMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(ZhiPuChatRequestWrapper.Create(model, messages, parameters),
                options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<ZhiPuChatResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<ChatMessage> messages,
        ChatParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(ZhiPuChatRequestWrapper.Create(model, messages, parameters),
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
                    var result = JsonSerializer.Deserialize<ZhiPuChatResponseWrapper>(data)!;
                    yield return (result.Choices[0].Delta.Content, result.Usage);
                }
                else if (line.StartsWith("{\"error\":"))
                {
                    throw new Exception(line);
                }
            }
        }
    }
}
