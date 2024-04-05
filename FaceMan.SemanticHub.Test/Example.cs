using Azure.AI.OpenAI;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;

using System.Threading;

namespace FaceMan.SemanticHub.Test
{
    [TestClass]
    public class Example
    {
        private readonly Kernel kernel;
        private ISemanticHubChatCompletionService _chatgpt;
        private ISemanticHubTextEmbeddingGenerationService _embeddingChatgpt;
        private ISemanticHubTextToImageService _imageChatgpt;
        private readonly SemanticHubOpenAIConfig inputOpenAI = new SemanticHubOpenAIConfig();
        private readonly SemanticHubAzureOpenAIConfig inputAzureOpenAI = new SemanticHubAzureOpenAIConfig()
        {
            Endpoint = "",
            ApiKey = "",
            DeploymentName = "text-embedding-ada-002"
        };
        private readonly SemanticHubXunFeiConfig inputXunFei = new SemanticHubXunFeiConfig();
        private readonly SemanticHubZhiPuConfig inputZhiPu = new SemanticHubZhiPuConfig();
        private readonly SemanticHubWenXinConfig inputWenXin = new SemanticHubWenXinConfig();
        private readonly SemanticHubTongYiConfig inputTongYi = new SemanticHubTongYiConfig()
        {
            ApiKey = "",
            ModelName = "wanx-v1",
            ImageParameters = new ImageParameters()
        };
        public Example()
        {
            //使用哪个注入哪个，重复注册会选择最后注册的。
            kernel = Kernel.CreateBuilder()
            //.AddSemanticHubXunFeiChatCompletion(inputXunFei)
            .AddSemanticHubAzureOpenAIChatCompletion(inputAzureOpenAI)
            //.AddSemanticHubOpenAIChatCompletion(inputOpenAI)
            //.AddSemanticHubTongYiChatCompletion(inputTongYi)
            //.AddSemanticHubWenXinChatCompletion(inputWenXin)
            //.AddSemanticHubZhiPuChatCompletion(inputZhiPu)
            .Build();
        }

        /// <summary>
        /// 使用Kernel.InvokeAsync()方法
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvokeAsync()
        {
            //对话
            //输出
            //你好
            string promptTemplate = @"
为给定的事件创造一个创造性的理由或借口。
要有创意，要风趣。让你的想象力天马行空。

事件：我要迟到了。
借口：我被长颈鹿歹徒勒索了赎金。

事件：我已经一年没去健身房了
借口：我一直忙于训练我的宠物龙。

事件：｛｛$input｝｝
";
            var excuseFunction = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings() { MaxTokens = 100, Temperature = 0.4, TopP = 1 });

            var result = await kernel.InvokeAsync(excuseFunction, new() { ["input"] = "我错过了F1决赛" });

            Console.WriteLine(result.GetValue<string>());
            //注意：每个模型供应商返回的值都不一样，详情请看代码
            Usage usage = (Usage)(result.Metadata?.GetValueOrDefault("Usage"));
            Console.WriteLine($"模型供应商：{result.Metadata?.GetValueOrDefault("Type")}，总消耗：{usage.TotalTokens}");
        }

        /// <summary>
        /// 生成文本
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTextContentsAsync()
        {
            _chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            var res = await _chatgpt.GetTextContentsAsync("帮我取个文静一点的花名");
            foreach (var item in res)
            {
                Console.WriteLine(item.Text);
                //注意：每个模型供应商返回的值都不一样，详情请看代码
                Usage usage = (Usage)(item.Metadata?.GetValueOrDefault("Usage"));
                Console.WriteLine($"模型供应商：{item.Metadata?.GetValueOrDefault("Type")}，总消耗：{usage.TotalTokens}");
            }
        }

        /// <summary>
        /// 生成文本-流式
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStreamingTextContentsAsync()
        {
            _chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            await foreach (var item in _chatgpt.GetStreamingTextContentsAsync("帮我取个文静一点的花名"))
            {
                Console.WriteLine(item.Text);
                //注意：每个模型供应商返回的值都不一样，详情请看代码
                Usage usage = (Usage)(item.Metadata?.GetValueOrDefault("Usage"));
                Console.WriteLine($"模型供应商：{item.Metadata?.GetValueOrDefault("Type")}，总消耗：{usage.TotalTokens}");
            }
        }

        /// <summary>
        /// 对话-流式
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStreamingChatMessageContentsAsync()
        {
            _chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            var chatHistory = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, "帮我取个文静一点的花名")
            };
            await foreach (var item in _chatgpt.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                Console.WriteLine(item.Content);
                //注意：每个模型供应商返回的值都不一样，详情请看代码
                Usage usage = (Usage)(item.Metadata?.GetValueOrDefault("Usage"));
                Console.WriteLine($"模型供应商：{item.Metadata?.GetValueOrDefault("Type")}，总消耗：{usage.TotalTokens}");
            }
        }

        /// <summary>
        /// 对话
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetChatMessageContentAsync()
        {
            _chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            var chatHistory = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, "帮我取个文静一点的花名")
            };
            var res = await _chatgpt.GetChatMessageContentsAsync(chatHistory);
            foreach (var item in res)
            {
                Console.WriteLine(item.Content);
                //注意：每个模型供应商返回的值都不一样，详情请看代码
                Usage usage = (Usage)(item.Metadata?.GetValueOrDefault("Usage"));
                Console.WriteLine($"模型供应商：{item.Metadata?.GetValueOrDefault("Type")}，总消耗：{usage.TotalTokens}");
            }

        }

        /// <summary>
        /// Embedding-向量化
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GenerateEmbeddingsAsync()
        {
            _embeddingChatgpt = new SemanticHubAzureOpenAIEmbeddingCompletionService(inputAzureOpenAI);

            var res = await _embeddingChatgpt.GenerateEmbeddingsAsync(new List<string> { "取个花名" });
            foreach (var item in res)
            {
                Console.WriteLine(item.Span[0]);
            }
        }

        /// <summary>
        /// Embedding-向量化-带token消耗
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GenerateEmbeddingsByUsageAsync()
        {
            _embeddingChatgpt = new SemanticHubAzureOpenAIEmbeddingCompletionService(inputAzureOpenAI);

            var res = await _embeddingChatgpt.GenerateEmbeddingsByUsageAsync(new List<string> { "取个花名" });
            foreach (var item in res.Item1)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine($"总消耗{res.Item2.TotalTokens}");
        }

        /// <summary>
        /// 生成图片
        /// 目前仅对接通义万象，因为Azure官方接口并没有返回消耗的Token信息，所以用SK自带的即可。
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GenerateImageAsync()
        {
            _imageChatgpt = new SemanticHubTongYiImageCompletionService(inputTongYi);
            //万象的参数不一样，为了方便注入ITextToImageService包装了一层这里的ImageGenerationOptions只是传参不用传值，在config里的ImageParameters传值即可
            var imageParameters = new ImageGenerationOptions();
            var res = await _imageChatgpt.GenerateImageAsync("奔跑的小男孩和小女孩", null);
            foreach (var item in res)
            {
                Console.WriteLine($"{item.ImageUrL}");
            }
        }
    }
}
