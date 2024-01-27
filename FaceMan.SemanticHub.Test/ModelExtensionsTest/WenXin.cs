using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;
using FaceMan.SemanticHub.ModelExtensions.WenXin.Chat;
using FaceMan.SemanticHub.ModelExtensions.WenXin.Image;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{

    [TestClass]
    public class WenXin
    {
        private ChatHistory historys;
        private WenXinChatCompletionService chatgpt;
        private OpenAIPromptExecutionSettings settings;
        private WenXinImageCompletionService imageService;
        public WenXin()
        {
            historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            chatgpt = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            imageService = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:自定义代理地址，可不填");
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
            //你好！有什么
            var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsAsync()
        {
            //流式
            //输出
            //你好！有什么
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
            //您好，
            //总消耗token：3 ,入参消耗token：1,出参消耗token：2
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

        //[TestMethod]
        //public async Task GetImageMessageContentsAsync()
        //{
        //    var parameters = new ImageParameters()
        //    {
        //    };
        //    var imgUrl = await imageService.GetImageMessageContentsAsync("画一只小清新风格的鲸鱼", parameters);
        //    foreach (var item in imgUrl)
        //    {
        //        Console.WriteLine($"生成的ImgUrl：{item}");
        //    }
        //}
    }
}
