// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

namespace FaceMan.SemanticHub.ModelExtensions.QianWen
{
	public class QianWenChatCompletionService : IModelExtensionsChatCompletionService
	{
		private readonly string _apiKey;
		private readonly string _model;
		public QianWenChatCompletionService(string key, string model)
		{
			_apiKey = key;
			_model = model;
		}

		public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
		{
			var histroyList = new List<ChatMessage>();
			ChatParameters chatParameters = null;
			foreach (var item in chatHistory)
			{
				var history = new ChatMessage()
				{
					Role = item.Role.Label,
					Content = item.Content,
				};
				histroyList.Add(history);
			}
			if (settings != null)
			{
				chatParameters = new ChatParameters()
				{
					TopP = settings != null ? (float)settings.TopP : default,
					MaxTokens = settings != null ? settings.MaxTokens : default,
					Temperature = settings != null ? (float)settings.Temperature : default,
					Seed = settings.Seed != null ? (ulong)settings.Seed : default,
					Stop = settings != null ? settings.StopSequences : default,
					//RepetitionPenalty = (float)settings.FrequencyPenalty,
					//TopK = (int)settings.PresencePenalty
				};
			}
			ModelClient client = new(_apiKey, ModelType.QianWen);
			QianWenResponseWrapper result = await client.QianWen.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
			var message = new ChatMessageContent(AuthorRole.Assistant, result.Output.Text);
			return message;
		}

		public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
		{
			var histroyList = new List<ChatMessage>();
			ChatParameters chatParameters = null;
			foreach (var item in chatHistory)
			{
				var history = new ChatMessage()
				{
					Role = item.Role.Label,
					Content = item.Content,
				};
				histroyList.Add(history);
			}
			if (settings != null)
			{
				chatParameters = new ChatParameters()
				{
					TopP = settings != null ? (float)settings.TopP : default,
					MaxTokens = settings != null ? settings.MaxTokens : default,
					Temperature = settings != null ? (float)settings.Temperature : default,
					Seed = settings.Seed != null ? (ulong)settings.Seed : default,
					Stop = settings != null ? settings.StopSequences : default,
				};
			}
			ModelClient client = new(_apiKey, ModelType.QianWen);

			await foreach (string item in client.QianWen.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
			{
				yield return item;
			}
		}

	}

}
