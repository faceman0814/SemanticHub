using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Chat;
using FaceMan.SemanticHub.ModelExtensions.TongYi.Image;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{

    [TestClass]
    public class TongYi
    {
        private ChatHistory historys;
        private TongYiChatCompletionService chatgpt;
        private OpenAIPromptExecutionSettings settings;
        private TongYiImageCompletionService imageService;
        public TongYi()
        {
            historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            var input = new SemanticHubTongYiConfig()
            {
                ApiKey = "sk-1e2853e50be14bba93f9a612aa71bb15",
                ModelName = "qwen-turbo"
            };
            chatgpt = new(input);
            settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 100,
                //....其他参数
            };
            imageService = new TongYiImageCompletionService("YourKey", "YourModel");
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
        public async Task GetImageMessageContentsAsync()
        {
            var parameters = new ImageParameters()
            {
                ImageStyle = StyleEnum.Auto
            };
            var imgUrl = await imageService.GetImageMessageContentsAsync("画一只小清新风格的鲸鱼", parameters);
            foreach (var item in imgUrl)
            {
                Console.WriteLine($"生成的ImgUrl：{item}");
            }
        }
    }
}
