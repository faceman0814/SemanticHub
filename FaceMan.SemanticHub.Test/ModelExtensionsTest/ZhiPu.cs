using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu.Chat;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{
    [TestClass]
    public class ZhiPu
    {
        private ChatHistory historys;
        private SemanticHubZhiPuChatCompletionService chatgpt;
        private OpenAIPromptExecutionSettings settings;

        public ZhiPu()
        {
            historys = new ChatHistory();
            historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            historys.AddUserMessage("用c#写一个冒泡排序");
            var input = new SemanticHubZhiPuConfig()
            {
                Secret = "",
                ModelName = "chatglm_turbo"
            };
            chatgpt = new(input);
            settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 100,
                //....其他参数
            };
        }

        [TestMethod]
        public async Task GetChatMessageContentsAsync()
        {
            //对话
            //输出
            //你好
            var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            Console.WriteLine(result[0].Content);
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsAsync()
        {
            //流式
            //输出
            //你好
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item.Content);
            }
        }
    }
}
