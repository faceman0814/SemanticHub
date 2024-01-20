﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
	/// <summary>
	/// 请求包装器
	/// </summary>
	public record QianWenRequestWrapper
	{
		public static QianWenRequestWrapper<TInput, TParameters> Create<TInput, TParameters>(string model, TInput input, TParameters? parameters = default) => new()
		{
			Model = model ?? throw new ArgumentNullException(nameof(model)),
			Input = input ?? throw new ArgumentNullException(nameof(input)),
			Parameters = parameters,
		};

		public static QianWenRequestWrapper<TInput, object> Create<TInput>(string model, TInput inputPrompt) => new()
		{
			Model = model ?? throw new ArgumentNullException(nameof(model)),
			Input = inputPrompt ?? throw new ArgumentNullException(nameof(inputPrompt)),
		};
	}

	public record QianWenRequestWrapper<TInput, TParameters> : QianWenRequestWrapper
	{
		[JsonPropertyName("model")]
		public string Model { get; set; }

		[JsonPropertyName("input")]
		public TInput Input { get; init; }

		[JsonPropertyName("parameters")]
		public TParameters? Parameters { get; init; }
	}
}
