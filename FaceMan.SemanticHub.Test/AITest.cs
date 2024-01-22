

using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.WenXin;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using AzureOpenAIChatCompletionService = FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureOpenAIChatCompletionService;
using OpenAIChatCompletionService = FaceMan.SemanticHub.ModelExtensions.OpenAI.OpenAIChatCompletionService;

namespace FaceMan.SemanticHub.Test
{
    [TestClass]
    public class AITest
    {
        [TestMethod]
        public async Task OpenAIChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            OpenAIChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:不填默认https://api.openai.com/v1");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task AzureOpenAIChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            AzureOpenAIChatCompletionService chatgpt = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:不填默认2023-07-01-preview");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task QianWenChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            QianWenChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task WenXinChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            WenXinChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task XunFeiChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            XunFeiChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourAppId", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task ZhiPuChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            ZhiPuChatCompletionService chatgpt = new("YourSecret", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....其他参数
            };

            //流式
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }
    }
}