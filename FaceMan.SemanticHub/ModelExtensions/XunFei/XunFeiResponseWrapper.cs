// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;

using System.Text.Json.Serialization;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
    public record XunFeiResponseWrapper
	{
		/// <summary>
		/// 请求头，包含请求结果，可以用来看是否请求成功
		/// </summary>
		[JsonPropertyName("header")]
		public ResponseHeader Header { get; set; }

		/// <summary>
		/// 负载
		/// </summary>
		[JsonPropertyName("payload")]
		public ResponsePayload Payload { get; set; }
	}

	/// <summary>
	/// 请求头
	/// </summary>
	public record ResponseHeader
	{
		/// <summary>
		/// 错误码，0表示正常，非0表示出错；详细释义可在接口说明文档最后的错误码说明了解
		/// </summary>
		[JsonPropertyName("code")]
		public int Code { get; set; }
		/// <summary>
		/// 会话是否成功的描述信息
		/// </summary>
		[JsonPropertyName("message")]
		public string Message { get; set; }
		/// <summary>
		/// 会话的唯一id，用于讯飞技术人员查询服务端会话日志使用,出现调用错误时建议留存该字段
		/// </summary>
		[JsonPropertyName("sid")]
		public string Sid { get; set; }
		/// <summary>
		/// 会话状态，取值为[0,1,2]；0代表首次结果；1代表中间结果；2代表最后一个结果
		/// </summary>
		[JsonPropertyName("status")]
		public string Status { get; set; }
	}

	/// <summary>
	/// 负载
	/// </summary>
	public record ResponsePayload
	{
		[JsonPropertyName("choices")]
		public Choices Choices { get; set; }

		[JsonPropertyName("usage")]
		public XunFeiUsage Usage { get; set; }
	}

	/// <summary>
	/// 选择
	/// </summary>
	public record Choices
	{
		/// <summary>
		/// 文本响应状态，取值为[0,1,2]; 0代表首个文本结果；1代表中间文本结果；2代表最后一个文本结果
		/// </summary>
		[JsonPropertyName("status")]
		public int Status { get; set; }
		/// <summary>
		/// 返回的数据序号，取值为[0,9999999]
		/// </summary>
		[JsonPropertyName("seq")]
		public int Seq { get; set; }
		/// <summary>
		/// 回复内容
		/// </summary>
		[JsonPropertyName("text")]
		public List<ResponseText> Text { get; set; }
	}

	/// <summary>
	/// 回复内容
	/// </summary>
	public record ResponseText
	{
		/// <summary>
		/// AI的回答内容
		/// </summary>
		[JsonPropertyName("content")]
		public string Content { get; set; }
		/// <summary>
		/// 角色标识，固定为assistant，标识角色为AI
		/// </summary>
		[JsonPropertyName("role")]
		public string Role { get; set; }
		/// <summary>
		/// 结果序号，取值为[0,10]; 当前为保留字段，开发者可忽略
		/// </summary>
		[JsonPropertyName("index")]
		public int Index { get; set; }
	}
	/// <summary>
	/// 在最后一次结果返回
	/// </summary>
	public record XunFeiUsage
    {
		/// <summary>
		/// 计算token的信息
		/// </summary>
		[JsonPropertyName("text")]
		public UsageText Text { get; set; }
	}

	/// <summary>
	/// 计算token的信息
	/// </summary>
	public record UsageText
	{
		/// <summary>
		/// 保留字段，可忽略
		/// </summary>
		[JsonPropertyName("question_tokens")]
		public int question_tokens { get; set; }
		/// <summary>
		/// 包含历史问题的总tokens大小
		/// </summary>
		[JsonPropertyName("prompt_tokens")]
		public int prompt_tokens { get; set; }
		/// <summary>
		/// 回答的tokens大小
		/// </summary>
		[JsonPropertyName("completion_tokens")]
		public int completion_tokens { get; set; }
		/// <summary>
		/// prompt_tokens和completion_tokens的和，也是本次交互计费的tokens大小
		/// </summary>
		[JsonPropertyName("total_tokens")]
		public int total_tokens { get; set; }
	}
}
