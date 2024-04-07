using Azure.AI.OpenAI;

using FaceMan.SemanticHub.Generation.ChatGeneration;
using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion;
using FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;
using FaceMan.SemanticHub.ModelExtensions.WenXin.Chat;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat;
using FaceMan.SemanticHub.Service.ChatCompletion;

using Google.Apis.CustomSearchAPI.v1.Data;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;

using System.Threading;

namespace FaceMan.SemanticHub.Test
{
    [TestClass]
    public class Example
    {
        private readonly Kernel kernel;
        private readonly OpenAIPromptExecutionSettings settings;
        private ISemanticHubChatCompletionService _chatgpt;
        private ISemanticHubTextEmbeddingGenerationService _embeddingChatgpt;
        private ISemanticHubTextToImageService _imageChatgpt;
        private readonly SemanticHubOpenAIConfig inputOpenAI = new SemanticHubOpenAIConfig()
        {
            Endpoint = "",
            ApiKey = "",
            ModelName = ""
        };
        private readonly SemanticHubAzureOpenAIConfig inputAzureOpenAI = new SemanticHubAzureOpenAIConfig()
        {
            Endpoint = "",
            ApiKey = "",
            //DeploymentName = "gpt-35-turbo"
            //DeploymentName = "text-embedding-ada-002"
            DeploymentName = ""
        };
        private readonly SemanticHubXunFeiConfig inputXunFei = new SemanticHubXunFeiConfig()
        {
            AppId = "",
            Secret = "",
            ApiKey = "",
            ModelName = ""
        };
        private readonly SemanticHubZhiPuConfig inputZhiPu = new SemanticHubZhiPuConfig()
        {
            Secret = "",
            ModelName = ""
        };
        private readonly SemanticHubWenXinConfig inputWenXin = new SemanticHubWenXinConfig()
        {
            ModelName = "ernie-bot-4",
            Secret = "",
            ApiKey = ""
        };
        private readonly SemanticHubTongYiConfig inputTongYi = new SemanticHubTongYiConfig()
        {
            ApiKey = "",
            //ModelName = "qwen-turbo",
            ModelName = "wanx-v1",
            ImageParameters = new ImageParameters()
        };
        public Example()
        {
            //使用哪个注入哪个，重复注册会选择最后注册的。
            kernel = Kernel.CreateBuilder()
            //.AddSemanticHubXunFeiChatCompletion(inputXunFei)
            //.AddSemanticHubAzureOpenAIChatCompletion(inputAzureOpenAI)
            .AddSemanticHubOpenAIChatCompletion(inputOpenAI)
            //.AddSemanticHubTongYiChatCompletion(inputTongYi)
            //.AddSemanticHubWenXinChatCompletion(inputWenXin)
            //.AddSemanticHubZhiPuChatCompletion(inputZhiPu)
            .Build();

            settings = new OpenAIPromptExecutionSettings() { MaxTokens = 10, Temperature = 0.4, TopP = 1 };
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

事件：{{$input}}
";
            var excuseFunction = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings() { MaxTokens = 512, Temperature = 0.4, TopP = 1 });

            var result = await kernel.InvokeAsync(excuseFunction, new() { ["input"] = "我错过了F1决赛" });

            Console.WriteLine(result.GetValue<string>());

            Result(result.Metadata);

        }

        /// <summary>
        /// 生成文本
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTextContentsAsync()
        {
            //_chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            //_chatgpt = new SemanticHubOpenAIChatCompletionService(inputOpenAI);
            //_chatgpt = new SemanticHubWenXinChatCompletionService(inputWenXin);
            //_chatgpt = new SemanticHubXunFeiChatCompletionService(inputXunFei);
            //_chatgpt = new SemanticHubZhiPuChatCompletionService(inputZhiPu);
            _chatgpt = new SemanticHubTongYiChatCompletionService(inputTongYi);
            var res = await _chatgpt.GetTextContentsAsync("帮我取个文静一点的花名");
            foreach (var item in res)
            {
                Console.WriteLine(item.Text);
                Result(item.Metadata);
            }
        }

        /// <summary>
        /// 生成文本-流式
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStreamingTextContentsAsync()
        {
            //_chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            //_chatgpt = new SemanticHubOpenAIChatCompletionService(inputOpenAI);
            //_chatgpt = new SemanticHubWenXinChatCompletionService(inputWenXin);
            //_chatgpt = new SemanticHubXunFeiChatCompletionService(inputXunFei);
            //_chatgpt = new SemanticHubZhiPuChatCompletionService(inputZhiPu);
            _chatgpt = new SemanticHubTongYiChatCompletionService(inputTongYi);
            await foreach (var item in _chatgpt.GetStreamingTextContentsAsync("帮我取个文静一点的花名", settings))
            {
                Console.WriteLine(item.Text);
                Result(item.Metadata);
            }
        }

        /// <summary>
        /// 对话-流式
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStreamingChatMessageContentsAsync()
        {
            //_chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            //_chatgpt = new SemanticHubOpenAIChatCompletionService(inputOpenAI);
            //_chatgpt = new SemanticHubWenXinChatCompletionService(inputWenXin);
            //_chatgpt = new SemanticHubXunFeiChatCompletionService(inputXunFei);
            //_chatgpt = new SemanticHubZhiPuChatCompletionService(inputZhiPu);
            _chatgpt = new SemanticHubTongYiChatCompletionService(inputTongYi);
            var chatHistory = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, "帮我取个文静一点的花名")
            };
            await foreach (var item in _chatgpt.GetStreamingChatMessageContentsAsync(chatHistory, settings))
            {
                Console.WriteLine(item.Content);
                Result(item.Metadata);
            }
        }

        /// <summary>
        /// 对话
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetChatMessageContentAsync()
        {
            //_chatgpt = new SemanticHubAzureOpenAIChatCompletionService(inputAzureOpenAI);
            //_chatgpt = new SemanticHubOpenAIChatCompletionService(inputOpenAI);
            //_chatgpt = new SemanticHubWenXinChatCompletionService(inputWenXin);
            //_chatgpt = new SemanticHubXunFeiChatCompletionService(inputXunFei);
            //_chatgpt = new SemanticHubZhiPuChatCompletionService(inputZhiPu);
            _chatgpt = new SemanticHubTongYiChatCompletionService(inputTongYi);
            var chatHistory = new ChatHistory()
            {
                new ChatMessageContent(AuthorRole.User, "帮我取个文静一点的花名")
            };
            var res = await _chatgpt.GetChatMessageContentsAsync(chatHistory, settings);
            foreach (var item in res)
            {
                Console.WriteLine(item.Content);
                Result(item.Metadata);
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
            //_imageChatgpt = new SemanticHubAzureOpenAIImageCompletionService(inputAzureOpenAI);
            //通义万象万象的参数不一样，为了实现ITextToImageService包装了一层，除模型和prompt需要在ImageGenerationOptions传值，其他参数在config里的ImageParameters传值即可。

            var imageParameters = new ImageGenerationOptions() //AI画画 Prompt和DeploymentName必填
            {
                DeploymentName = inputAzureOpenAI.DeploymentName,
                Prompt = "奔跑的小男孩"
            };
            var res = await _imageChatgpt.GenerateImageAsync(imageParameters);

            foreach (var item in res)
            {
                Console.WriteLine($"{item.ImageUrL}");
            }
        }

        private void Result(IReadOnlyDictionary<string, object?>? Metadata, bool IsStream = false)
        {
            var type = Metadata?["Type"].ToString();

            //注意：每个模型供应商返回的值都不一样，详情请看代码
            switch (type)
            {
                case "XunFei":
                    ResponsePayload payload = (ResponsePayload)(Metadata?.GetValueOrDefault("Payload"));
                    Console.WriteLine($"总消耗：{payload?.Usage?.Text?.total_tokens}");
                    break;
                case "AzureOpenAI":
                    Usage usage = (Usage)(Metadata?.GetValueOrDefault("Usage"));
                    Console.WriteLine($"总消耗：{usage?.TotalTokens}");
                    break;
                case "OpenAI":
                    Usage openAIUsage = (Usage)(Metadata?.GetValueOrDefault("Usage"));
                    Console.WriteLine($"总消耗：{openAIUsage?.TotalTokens}");
                    break;
                case "TongYi":
                    QianWenUsage qianWenUsage = (QianWenUsage)(Metadata?.GetValueOrDefault("Usage"));
                    Console.WriteLine($"总消耗：{qianWenUsage?.TotalTokens}");
                    break;
                case "ZhiPu":
                    Usage zhiPuUsage = (Usage)(Metadata?.GetValueOrDefault("Usage"));
                    Console.WriteLine($"总消耗：{zhiPuUsage?.TotalTokens}");
                    break;
                case "WenXin":
                    Usage wenXinUsage = (Usage)(Metadata?.GetValueOrDefault("Usage"));
                    Console.WriteLine($"总消耗：{wenXinUsage?.TotalTokens}");
                    break;
            }
        }
    }
}
