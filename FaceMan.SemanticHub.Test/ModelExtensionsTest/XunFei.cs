using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.Chat;
using FaceMan.SemanticHub.ModelExtensions.XunFei;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{
    [TestClass]
    public class XunFei
    {
        private ChatHistory historys;
        private XunFeiChatCompletionService chatgpt;
        private OpenAIPromptExecutionSettings settings;

        public XunFei()
        {
            historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            chatgpt = new("YourKey", "YourSecret", "YourAppId", "YourModel", "YourEndPoint:自定义代理地址，可不填");
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
            //你好！有什么
            //总消耗token：13 ,入参消耗token：10,出参消耗token：3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsByTokenAsync()
        {
            //流式————返回token
            //输出
            //你好！有什么总消耗token：13 ,入参消耗token：10,出参消耗token：3
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
    }

}
