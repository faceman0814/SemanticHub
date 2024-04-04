// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.WenXin;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

using SemanticHubOpenAIClient = FaceMan.SemanticHub.ModelExtensions.OpenAI.SemanticHubOpenAIClient;

namespace FaceMan.SemanticHub.ModelExtensions
{
    public class ModelClient : IDisposable
    {
        internal readonly HttpClient HttpClient = null!;

        public ModelClient(string apiKey, ModelType modelType, string url, HttpClient? httpClient = null, string apiVersion = null)
        {
            HttpClient = httpClient ?? new HttpClient();
            switch (modelType)
            {
                case ModelType.ZhiPu:
                    int expirationInSeconds = 3600; // 设置过期时间为1小时
                    apiKey = GenerateJwtToken(apiKey, expirationInSeconds);
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;
                case ModelType.TongYi:
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;
                case ModelType.XunFei:
                    break;
                case ModelType.AzureOpenAI:
                    HttpClient.DefaultRequestHeaders.Add("api-key", apiKey);
                    break;
                case ModelType.OpenAI:
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    break;
            }
            TongYi = new TongYiClient(this, url);
            ZhiPu = new ZhiPuClient(this, url);
            XunFei = new XunFeiClient(this, url);
            WenXin = new WenXinClient(this, url);
            AzureOpenAI = new SemanticHubAzureOpenAIClient(this, url, apiVersion);
            OpenAI = new SemanticHubOpenAIClient(this, url);
        }

        public TongYiClient TongYi { get; set; }
        public ZhiPuClient ZhiPu { get; set; }
        public XunFeiClient XunFei { get; set; }
        public WenXinClient WenXin { get; set; }
        public SemanticHubAzureOpenAIClient AzureOpenAI { get; set; }
        public SemanticHubOpenAIClient OpenAI { get; set; }
        /// <summary>
        /// 处理基础HTTP客户端。
        /// </summary>
        public void Dispose() => HttpClient.Dispose();

        /// <summary>
        /// 数据流转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">响应体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<T> ReadResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            try
            {
                var debug = await response.Content.ReadAsStringAsync();
                return (await response.Content.ReadFromJsonAsync<T>(options: null, cancellationToken))!;
            }
            catch (Exception e) when (e is NotSupportedException or System.Text.Json.JsonException)
            {
                throw new Exception($"未能将以下json转换为: {typeof(T).Name}: {await response.Content.ReadAsStringAsync()}", e);
            }
        }

        public static async Task<T> ReadImageResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return (await ReadResponse<ImageResponseWrapper<T, ImageTaskUsage>>(response, cancellationToken)).Output;
        }
        /// <summary>
        /// 讯飞星火 数据流转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="receivedMessage"></param>
        /// <returns></returns>
        public static XunFeiChatResponseWrapper ReadResponse<T>(string receivedMessage)
        {
            XunFeiChatResponseWrapper response = JsonConvert.DeserializeObject<XunFeiChatResponseWrapper>(receivedMessage);
            return response;
        }

        /// <summary>
        /// 智谱生成JWT令牌
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="expSeconds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal string GenerateJwtToken(string apiKey, int expSeconds)
        {
            // 分割API Key以获取ID和Secret
            var parts = apiKey.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid API key format.");
            }

            var id = parts[0];
            var secret = parts[1];

            // 创建Header信息
            var header = new JwtHeader(new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256))
                {
                    {"sign_type", "SIGN"}
                };

            // 创建Payload信息
            long currentMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var payload = new JwtPayload
            {
                {"api_key", id},
                {"exp", currentMillis + expSeconds * 1000},
                {"timestamp", currentMillis}
            };

            // 生成JWT Token
            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
