// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using FaceMan.SemanticHub.ModelExtensions.QianWen;

namespace FaceMan.SemanticHub.ModelExtensions.XunFei
{
	public record XunFeiRequestWrapper
	{
		public string AppId { get; set; }
		public string Secret { get; set; }
		public string key { get; set; }
	}
	public record XunFeiRequest
	{
		public Header header { get; set; }
		public Parameter parameter { get; set; }
		public Payload payload { get; set; }
	}

	public record Header
	{
		public string app_id { get; set; }
		public string uid { get; set; }
	}

	public record Parameter
	{
		public Chat chat { get; set; }
	}

	public record Chat
	{
		public string domain { get; set; }
		public double temperature { get; set; }
		public int max_tokens { get; set; }
		public double top_k { get; set; }
	}

	public record Payload
	{
		public Message message { get; set; }
	}

	public record Message
	{
		public List<Content> text { get; set; }
	}

	public record Content
	{
		public string role { get; set; }
		public string content { get; set; }
	}
}
