// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

using Usage = FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat.Usage;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
    public class XunFeiClient
    {
        private ClientWebSocket webSocket0;
        private readonly string baseUrl = "https://spark-api.xf-yun.com/v1.1/chat";

        internal XunFeiClient(ModelClient parent, string url = null)
        {
            Parent = parent;
            baseUrl = url ?? baseUrl;
        }
        internal ModelClient Parent { get; }

        public async Task<(string, Usage)> GetChatMessageContentsAsync(XunFeiRequest request, XunFeiRequestWrapper xunFeiRequest, CancellationToken cancellationToken = default)
        {
            string authUrl = GetAuthUrl(xunFeiRequest.Secret, xunFeiRequest.key);
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                await webSocket0.ConnectAsync(new Uri(url), cancellationToken);
                string jsonString = JsonConvert.SerializeObject(request);
                //连接成功，开始发送数据

                var frameData2 = System.Text.Encoding.UTF8.GetBytes(jsonString.ToString());
                await webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellationToken);
                // 接收流式返回结果进行解析
                byte[] receiveBuffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);
                String resp = "";
                Usage usage = new Usage();
                while (!result.CloseStatus.HasValue)
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                        //将结果构造为json
                        var response = ModelClient.ReadResponse<XunFeiResponseWrapper>(receivedMessage);

                        if (response.Header.Code == 0)
                        {
                            int status = response.Payload.Choices.Status;
                            string content = response.Payload.Choices.Text[0].Content;
                            resp += content;
                            if (status == 2)
                            {
                                usage.PromptTokens += response.Payload.Usage.Text.prompt_tokens;
                                usage.TotalTokens += response.Payload.Usage.Text.total_tokens;
                                usage.CompletionTokens += response.Payload.Usage.Text.completion_tokens;
                                break;
                            }
                        }
                        else
                        {
                            throw new Exception(receivedMessage);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("已关闭WebSocket连接");
                        break;
                    }
                    result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);
                }
                return (resp, usage);

            }
        }

        public async IAsyncEnumerable<(string, Usage)> GetStreamingChatMessageContentsAsync(XunFeiRequest request, XunFeiRequestWrapper xunFeiRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            string authUrl = GetAuthUrl(xunFeiRequest.Secret, xunFeiRequest.key);
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                await webSocket0.ConnectAsync(new Uri(url), cancellationToken);
                string jsonString = JsonConvert.SerializeObject(request);
                //连接成功，开始发送数据

                var frameData2 = System.Text.Encoding.UTF8.GetBytes(jsonString.ToString());
                await webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellationToken);
                // 接收流式返回结果进行解析
                byte[] receiveBuffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);
                Usage usage = new Usage();
                while (!result.CloseStatus.HasValue)
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                        //将结果构造为json
                        var response = ModelClient.ReadResponse<XunFeiResponseWrapper>(receivedMessage);

                        if (response.Header.Code == 0)
                        {
                            int status = response.Payload.Choices.Status;
                            string content = response.Payload.Choices.Text[0].Content;
                            if (status == 2)
                            {
                                usage.PromptTokens += response.Payload.Usage.Text.prompt_tokens;
                                usage.TotalTokens += response.Payload.Usage.Text.total_tokens;
                                usage.CompletionTokens += response.Payload.Usage.Text.completion_tokens;
                                yield return (null, usage);
                                break;
                            }
                            else
                            {
                                yield return (content, usage);
                            }
                        }
                        else
                        {
                            throw new Exception(receivedMessage);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("已关闭WebSocket连接");
                        break;
                    }
                    result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);
                }
            }
        }

        /// <summary>
        /// 获取权限路由
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetAuthUrl(string secret, string key)
        {
            string date = DateTime.UtcNow.ToString("r");

            Uri uri = new Uri(baseUrl);
            StringBuilder builder = new StringBuilder("host: ").Append(uri.Host).Append("\n").//
                                    Append("date: ").Append(date).Append("\n").//
                                    Append("GET ").Append(uri.LocalPath).Append(" HTTP/1.1");

            string sha = HMACsha256(secret, builder.ToString());
            string authorization = string.Format("api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"", key, "hmac-sha256", "host date request-line", sha);

            string NewUrl = "https://" + uri.Host + uri.LocalPath;

            string path1 = "authorization" + "=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authorization));
            date = date.Replace(" ", "%20").Replace(":", "%3A").Replace(",", "%2C");
            string path2 = "date" + "=" + date;
            string path3 = "host" + "=" + uri.Host;

            NewUrl = NewUrl + "?" + path1 + "&" + path2 + "&" + path3;
            return NewUrl;
        }

        /// <summary>
        /// HMAC-SHA256算法对给定的字符串进行加密
        /// </summary>
        /// <param name="apiSecretIsKey"></param>
        /// <param name="buider"></param>
        /// <returns></returns>
        private string HMACsha256(string apiSecretIsKey, string buider)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(apiSecretIsKey);
            System.Security.Cryptography.HMACSHA256 hMACSHA256 = new System.Security.Cryptography.HMACSHA256(bytes);
            byte[] date = System.Text.Encoding.UTF8.GetBytes(buider);
            date = hMACSHA256.ComputeHash(date);
            hMACSHA256.Clear();
            return Convert.ToBase64String(date);
        }
    }
}
