// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;

using Microsoft.SemanticKernel;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
    public class TongYiClient
    {
        /// <summary>
        /// 基础请求地址
        /// </summary>
        private readonly string baseUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
        private readonly string ImgbaseUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text2image/image-synthesis";
        private readonly string ImgTaskbaseUrl = "https://dashscope.aliyuncs.com/api/v1/tasks/";
        internal TongYiClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
            ImgbaseUrl = url ?? ImgbaseUrl;
            ImgTaskbaseUrl = url ?? ImgTaskbaseUrl;
        }
        internal ModelClient Parent { get; }

        /// <summary>
        /// 生成对话
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="messages">上下文</param>
        /// <param name="parameters">模型参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TongYiChatResponseWrapper> GetChatMessageContentsAsync(string model, IReadOnlyList<ChatMessage> messages, ChatParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(TongYiChatRequestWrapper.Create(model, new
                {
                    messages,
                }, parameters), options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadResponse<TongYiChatResponseWrapper>(resp, cancellationToken);
        }

        /// <summary>
        /// 流式生成对话
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="messages">上下文</param>
        /// <param name="parameters">模型参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        public async IAsyncEnumerable<TongYiChatResponseWrapper> GetStreamingChatMessageContentsAsync(string model,
        IReadOnlyList<ChatMessage> messages,
        ChatParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, baseUrl)
            {
                Content = JsonContent.Create(TongYiChatRequestWrapper.Create(model, new
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
                    var result = JsonSerializer.Deserialize<TongYiChatResponseWrapper>(data)!;
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
                    result.Output.Text = addedText;
                    yield return result;
                }
            }
        }

        /// <summary>
        /// 生成图像
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="prompt">提示词</param>
        /// <param name="parameters">模型参数</param>
        /// <param name="kernel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TongYiImageResponseWrapper> GetImageMessageContentsAsync(string model, string prompt, ImageParameters parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(parameters.Size))
            {
                parameters.ImageSize = SizeEnum.Size1024x1024;
            }
            HttpRequestMessage httpRequest = new(HttpMethod.Post, ImgbaseUrl)
            {
                Content = JsonContent.Create(TongYiImageRequestWrapper.Create(model, new { prompt }, parameters), options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
            };
            httpRequest.Headers.TryAddWithoutValidation("X-DashScope-Async", "enable");
            HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
            return await ModelClient.ReadImageResponse<TongYiImageResponseWrapper>(resp, cancellationToken);
        }

        public async Task<TongYiImageTaskStatusResponseWrapper> QueryTaskStatus(string taskId, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage resp = await Parent.HttpClient.GetAsync(ImgTaskbaseUrl + taskId);
            return await ModelClient.ReadImageResponse<TongYiImageTaskStatusResponseWrapper>(resp, cancellationToken);
        }
    }
}
