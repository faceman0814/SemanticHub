// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DocumentFormat.OpenXml.EMMA;

using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Newtonsoft.Json;

namespace FaceMan.SemanticHub.ModelExtensions.WenXin
{
    public class WenXinChatCompletionService : IModelExtensionsChatCompletionService
    {
        private readonly string _secret;
        private readonly string _key;
        private readonly string _model;
        private readonly string _url;
        public WenXinChatCompletionService(string key, string secret, string model, string url = null)
        {
            _secret = secret;
            _model = model;
            _key = key;
            _url = url;
        }
        public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                if (item.Role == AuthorRole.System)
                {
                    continue;
                }
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            var token = await GetAccessToken();
            var system = chatHistory.FirstOrDefault(t => t.Role == AuthorRole.System);
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    Temperature = settings != null ? (float)settings.Temperature : default,
                    Stop = settings != null ? settings.StopSequences : default,
                    PenaltyScore = settings != null ? (float)settings.FrequencyPenalty : default,
                    System = system != null ? system.Content : default,
                    Token = token
                };
            }
            else
            {
                chatParameters = new ChatParameters()
                {
                    System = system != null ? system.Content : default,
                    Token = token
                };
            }
            ModelClient client = new(_secret, ModelType.WenXin, _url);
            var result = await client.WenXin.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
            return new ChatMessageContent(AuthorRole.Assistant, result.Result);
        }

        public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var histroyList = new List<ChatMessage>();
            ChatParameters chatParameters = null;
            foreach (var item in chatHistory)
            {
                if (item.Role == AuthorRole.System)
                {
                    continue;
                }
                var history = new ChatMessage()
                {
                    Role = item.Role.Label,
                    Content = item.Content,
                };
                histroyList.Add(history);
            }
            var token = await GetAccessToken();
            var system = chatHistory.FirstOrDefault(t => t.Role == AuthorRole.System);
            if (settings != null)
            {
                chatParameters = new ChatParameters()
                {
                    TopP = settings != null ? (float)settings.TopP : default,
                    Temperature = settings != null ? (float)settings.Temperature : default,
                    Stop = settings != null ? settings.StopSequences : default,
                    PenaltyScore = settings != null ? (float)settings.FrequencyPenalty : default,
                    System = system != null ? system.Content : default,
                    Token = token,
                    Stream = true
                };
            }
            else
            {
                chatParameters = new ChatParameters()
                {
                    System = system != null ? system.Content : default,
                    Token = token,
                    Stream = true
                };
            }
            ModelClient client = new(_secret, ModelType.WenXin, _url);
            await foreach (string item in client.WenXin.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
            {
                yield return item;
            }
        }

        /**
	   * 使用 AK，SK 生成鉴权签名（Access Token）
	   * @return 鉴权签名信息（Access Token）
	   */
        private async Task<string> GetAccessToken()
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
