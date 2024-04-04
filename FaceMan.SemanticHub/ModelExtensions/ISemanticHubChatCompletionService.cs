using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.Service.ChatCompletion
{
    public interface ISemanticHubChatCompletionService : IChatCompletionService, ITextGenerationService
    {
        //Task<(IReadOnlyList<TextContent>, Usage)> GetTextContentsByTokenAsync(string prompt, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
        //IAsyncEnumerable<(StreamingTextContent, Usage)> GetStreamingTextContentsByTokenAsync(string prompt, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
        //Task<(IReadOnlyList<ChatMessageContent>, Usage)> GetChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
        //IAsyncEnumerable<(StreamingChatMessageContent, Usage)> GetStreamingChatMessageContentsByTokenAsync(ChatHistory chatHistory, OpenAIPromptExecutionSettings settings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
    }
}
