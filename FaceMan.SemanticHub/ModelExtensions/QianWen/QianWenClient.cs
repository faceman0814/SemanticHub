// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.Drawing;

using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
    public class QianWenClient
    {
        /// <summary>
        /// 基础请求地址
        /// </summary>
        private readonly string baseUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
        internal QianWenClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
        }
        internal ModelClient Parent { get; }

        public async Task<QianWenResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<ChatMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(QianWenRequestWrapper.Create(model, new
                {
                    messages,
                }, parameters), options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<QianWenResponseWrapper>(resp, cancellationToken);
        }

        public async IAsyncEnumerable<(string, QianWenUsage)> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<ChatMessage> messages,
        ChatParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(QianWenRequestWrapper.Create(model, new
                {
                    messages,
                }, parameters), options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            httpRequest.Headers.TryAddWithoutValidation("X-DashScope-SSE", "enable");

            using HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }

            string lastText = string.Empty; // 记录上一次返回的数据
            using StreamReader reader = new(await resp.Content.ReadAsStreamAsync(), Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                string? line = await reader.ReadLineAsync();
                if (line != null && line.StartsWith("data:"))
                {
                    string data = line["data:".Length..];
                    if (data.StartsWith("{\"code\":"))
                    {
                        throw new Exception(data);
                    }
                    var result = JsonSerializer.Deserialize<QianWenResponseWrapper>(data)!;
                    // 获取新增加的部分数据并返回
                    int commonPrefixLength = 0;
                    while (commonPrefixLength < lastText.Length && commonPrefixLength < result.Output.Text.Length && lastText[commonPrefixLength] == data[commonPrefixLength])
                    {
                        commonPrefixLength++;
                    }
                    // 获取新增加的文本部分并返回
                    string newText = result.Output.Text;
                    string addedText = newText.Substring(lastText.Length);

                    lastText = newText;

                    yield return (addedText, result.Usage);
                }
            }
        }
    }
}
