using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Image;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using AzureOpenAIChatCompletionService = FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat.AzureOpenAIChatCompletionService;
using OpenAIChatCompletionService = FaceMan.SemanticHub.ModelExtensions.OpenAI.Chat.OpenAIChatCompletionService;

namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{
    [TestClass]
    public class AzureOpenAI
    {
        private ChatHistory historys;
        private AzureOpenAIChatCompletionService chatgpt;
        private AzureOpenAIImageCompletionService imageService;
        private OpenAIPromptExecutionSettings settings;

        public AzureOpenAI()
        {
            historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            chatgpt = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:不填默认2023-07-01-preview");
            imageService = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:不填默认2023-07-01-preview");
            imageService = new("b3d4d46e0e5847e19c690a58fe106fd9", "https://faceman.openai.azure.com", "Dalle3");
            settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };
        }

        [TestMethod]
        public async Task GetChatMessageContentsAsync()
        {
            //对话
            //输出
            //你好！
            var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsAsync()
        {
            //流式
            //输出
            //你好！
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public async Task GetChatMessageContentsByTokenAsync()
        {
            //对话————返回token
            //输出
            //你好！
            //总消耗token：12 ,入参消耗token：9,出参消耗token：3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsByTokenAsync()
        {
            //流式————返回token
            //输出
            //你好，总消耗token：4 ,入参消耗token：2,出参消耗token：2
            var sum = new Usage();
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsByTokenAsync(historys, settings))
            {
                Console.Write($"{item.Item1}");
                if (item.Item2 != null)
                {
                    sum.CompletionTokens += item.Item2.CompletionTokens;
                    sum.PromptTokens += item.Item2.PromptTokens;
                    sum.TotalTokens += item.Item2.TotalTokens;
                }
            }
            Console.Write($"总消耗token：{sum.TotalTokens} ,入参消耗token：{sum.PromptTokens},出参消耗token：{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task GetImageMessageContentsAsync()
        {
            var imgUrl = await imageService.GetImageMessageContentsAsync("画一只小清新风格的鲸鱼", 1024, 1024);
            Console.WriteLine($"生成的ImgUrl：{imgUrl}");
        }
    }
}
