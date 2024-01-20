// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.ModelExtensions.TextGeneration
{
	/// <summary>
	/// A single chat message within the conversation.
	/// </summary>
	public record ChatMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChatMessage"/> class.
		/// </summary>
		public ChatMessage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChatMessage"/> class with specified role and content.
		/// </summary>
		/// <param name="role">The role of the message sender.</param>
		/// <param name="content">The content of the message.</param>
		public ChatMessage(string role, string content)
		{
			Role = role;
			Content = content;
		}

		/// <summary>
		/// Create a chat message from the system with specified content.
		/// </summary>
		/// <param name="content">The content of the message.</param>
		/// <returns>A new <see cref="ChatMessage"/> instance representing a system message.</returns>
		public static ChatMessage FromSystem(string content) => new("system", content);

		/// <summary>
		/// Create a chat message from a user with specified content.
		/// </summary>
		/// <param name="content">The content of the message.</param>
		/// <returns>A new <see cref="ChatMessage"/> instance representing a user message.</returns>
		public static ChatMessage FromUser(string content) => new("user", content);

		/// <summary>
		/// Create a chat message from the assistant with specified content.
		/// </summary>
		/// <param name="content">The content of the message.</param>
		/// <returns>A new <see cref="ChatMessage"/> instance representing an assistant message.</returns>
		public static ChatMessage FromAssistant(string content) => new("assistant", content);

		/// <summary>
		/// Gets or sets the role of the message sender.
		/// The role indicates if the sender is a system, a user, or an assistant.
		/// </summary>
		[JsonPropertyName("role")]
		public string Role { get; set; }

		/// <summary>
		/// Gets or sets the content of the message.
		/// This is the actual text of the message sent by the sender.
		/// </summary>
		[JsonPropertyName("content")]
		public string Content { get; set; }
	}

}
