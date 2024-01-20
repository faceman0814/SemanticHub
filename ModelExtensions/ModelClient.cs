// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.WenXin;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

namespace FaceMan.SemanticHub.ModelExtensions
{
	public class ModelClient : IDisposable
	{
		internal readonly HttpClient HttpClient = null!;

		public ModelClient(string apiKey, ModelType modelType, HttpClient? httpClient = null)
		{
			HttpClient = httpClient ?? new HttpClient();
			switch (modelType)
			{
				case ModelType.ZhiPu:
					int expirationInSeconds = 3600; // 设置过期时间为1小时
					apiKey = GenerateJwtToken(apiKey, expirationInSeconds);
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
					break;
				case ModelType.QianWen:
					HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
					break;
				case ModelType.XunFei:
					break;

			}
			QianWen = new QianWenClient(this);
			ZhiPu = new ZhiPuClient(this);
			XunFei = new XunFeiClient(this);
			WenXin = new WenXinClient(this);
		}

		public QianWenClient QianWen { get; set; }
		public ZhiPuClient ZhiPu { get; set; }
		public XunFeiClient XunFei { get; set; }
		public WenXinClient WenXin { get; set; }
		/// <summary>
		/// 处理基础HTTP客户端。
		/// </summary>
		public void Dispose() => HttpClient.Dispose();

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
		public static XunFeiResponseWrapper ReadResponse<T>(string receivedMessage)
		{
			XunFeiResponseWrapper response = JsonConvert.DeserializeObject<XunFeiResponseWrapper>(receivedMessage);
			return response;
		}

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
