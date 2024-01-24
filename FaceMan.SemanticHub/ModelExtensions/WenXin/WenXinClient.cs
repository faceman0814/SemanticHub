// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.Generation.TextGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.WenXin.Chat;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using Microsoft.SemanticKernel;

using Newtonsoft.Json;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin
{
    public class WenXinClient
    {
        private readonly string baseUrl = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/";
        private readonly string ImgBaseUrl = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/text2image/";
        internal WenXinClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
            ImgBaseUrl = url ?? ImgBaseUrl;
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

        public async Task<List<string>> GetImageMessageContentsAsync(string model, string prompt, ImageParameters parameters, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Post, ImgBaseUrl + model + $"?access_token={parameters.Token}")
            {
                // Content = JsonContent.Create(WenXinRequestWrapper.Create(messages, parameters),
                //options: new JsonSerializerOptions
                //{
                //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                //}),
            };
            return default;
        }

        /**
      * 使用 AK，SK 生成鉴权签名（Access Token）
      * @return 鉴权签名信息（Access Token）
      */
        public async Task<string> GetAccessToken(string _key, string _secret)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://aip.baidubce.com/oauth/2.0/token");
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            parameters.Add(new KeyValuePair<string, string>("client_id", _key));
            parameters.Add(new KeyValuePair<string, string>("client_secret", _secret));
            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content);
            return result.access_token.ToString();
        }
    }
}
