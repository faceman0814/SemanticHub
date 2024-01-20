// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.TextGeneration;

namespace FaceMan.SemanticHub.ModelExtensions.ZhiPu
{
	public class ZhiPuChatCompletionService : IModelExtensionsChatCompletionService
	{
		private readonly string _secret;
		private readonly string _model;
		public ZhiPuChatCompletionService(string secret, string model)
		{
			_secret = secret;
			_model = model;
		}
		public async Task<ChatMessageContent> GetChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
		{
			var histroyList = new List<ChatMessage>();
			//因为智谱AI官方调用的有bug，所以这里做一下处理。
			histroyList.Add(ChatMessage.FromSystem("1"));
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
					// max_tokens 应该在 [1, 1500]的区间
					MaxTokens = settings != null ? settings.MaxTokens : default,
					Temperature = settings != null ? (float)settings.Temperature : default,
					Stop = settings != null ? settings.StopSequences : default
					//RepetitionPenalty = (float)settings.FrequencyPenalty,
					//TopK = (int)settings.PresencePenalty
				};
			}
			ModelClient client = new(_secret, ModelType.ZhiPu);
			var result = await client.ZhiPu.GetChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken);
			return new ChatMessageContent(AuthorRole.Assistant, result.Choices[0].Message.Content);
		}

		public async IAsyncEnumerable<string> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
		{
			var histroyList = new List<ChatMessage>();
			//因为智谱AI官方调用有bug，所以这里做一下处理。
			histroyList.Add(ChatMessage.FromSystem("1"));
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
					// max_tokens 应该在 [1, 1500]的区间
					MaxTokens = settings != null ? settings.MaxTokens : default,
					Temperature = settings != null ? (float)settings.Temperature : default,
					Stop = settings != null ? settings.StopSequences : default,
					Stream = true
				};
			}
			else
			{
				chatParameters = new ChatParameters()
				{
					Stream = true
				};
			}
			ModelClient client = new(_secret, ModelType.ZhiPu);

			await foreach (string item in client.ZhiPu.GetStreamingChatMessageContentsAsync(_model, histroyList, chatParameters, cancellationToken))
			{
				yield return item;
			}
		}
	}
}
