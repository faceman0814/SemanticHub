

using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI;
using FaceMan.SemanticHub.ModelExtensions.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.WenXin;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

using Google.Apis.CustomSearchAPI.v1.Data;

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
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            //OpenAIChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            OpenAIChatCompletionService chatgpt = new("sk-YX7bGSR3hMivBqVq42F35a146b214f46AcCf4c4f073c2d49", "gpt-3.5-turbo", "https://oneapi.faceman.cn/v1");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };

            ////对话
            ////输出
            ////你好！
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            ////流式
            ////输出
            ////你好！
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //对话————返回token
            //输出
            //你好！
            //总消耗token：12 ,入参消耗token：9,出参消耗token：3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");

            //流式————返回token
            //输出
            //你好！总消耗token：0 ,入参消耗token：0,出参消耗token：0————OpenAI流式接口不返回token消耗情况
            //var sum = new Usage();
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsByTokenAsync(historys, settings))
            //{
            //    Console.Write($"{item.Item1}");
            //    if (item.Item2 != null)
            //    {
            //        sum.CompletionTokens += item.Item2.CompletionTokens;
            //        sum.PromptTokens += item.Item2.PromptTokens;
            //        sum.TotalTokens += item.Item2.TotalTokens;
            //    }

            //}
            //Console.Write($"总消耗token：{sum.TotalTokens} ,入参消耗token：{sum.PromptTokens},出参消耗token：{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task AzureOpenAIChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            AzureOpenAIChatCompletionService chatgpt = new("28df2a037e7c432a85f7892d280b99c9", "https://yoyochatx.openai.azure.com/", "gpt-35-turbo");
            //AzureOpenAIChatCompletionService chatgpt = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:不填默认2023-07-01-preview");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };

            //对话
            //输出
            //你好！
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //流式
            //输出
            //你好！
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            ////对话————返回token
            ////输出
            ////你好！
            ////总消耗token：12 ,入参消耗token：9,出参消耗token：3
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");

            //流式————返回token
            //输出
            //你好！总消耗token：0 ,入参消耗token：0,出参消耗token：0
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
        public async Task QianWenChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            //QianWenChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            QianWenChatCompletionService chatgpt = new("sk-1e2853e50be14bba93f9a612aa71bb15", "qwen-turbo");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };
            //对话
            //输出
            //你好！有什么
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //流式
            //输出
            //你好！有什么
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //对话————返回token
            //输出
            //你好！有什么
            //总消耗token：4 ,入参消耗token：1,出参消耗token：3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");

            //流式————返回token
            //输出
            //你好！有什么总消耗token：8 ,入参消耗token：2,出参消耗token：6
            //var sum = new Usage();
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsByTokenAsync(historys, settings))
            //{
            //    Console.Write($"{item.Item1}");
            //    if (item.Item2 != null)
            //    {
            //        sum.CompletionTokens += item.Item2.CompletionTokens;
            //        sum.PromptTokens += item.Item2.PromptTokens;
            //        sum.TotalTokens += item.Item2.TotalTokens;
            //    }
            //}
            //Console.Write($"总消耗token：{sum.TotalTokens} ,入参消耗token：{sum.PromptTokens},出参消耗token：{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task WenXinChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            //WenXinChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:自定义代理地址，可不填");
            WenXinChatCompletionService chatgpt = new("oSoGsTeMD1OGVaaInpnj3U9U", "PAkyqMhAq7S6IQKftjcAcUAn1PGOA2yU", "completions");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };

            //对话
            //输出
            //你好！有什么
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //流式
            //输出
            //你好！有什么
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //对话————返回token
            //输出
            //您好，
            //总消耗token：3 ,入参消耗token：1,出参消耗token：2
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");

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
        public async Task XunFeiChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");
            historys.AddUserMessage("你好");
            //XunFeiChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourAppId", "YourModel:可不填，默认general", "YourEndPoint:自定义代理地址，可不填");

            XunFeiChatCompletionService chatgpt = new("a836acd63f6546265a843a3df0db1bc1", "NjBlYmNiOGMzNWQ3ODEwZGQ2NmY1MGJk", "1d29d888");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....其他参数
            };

            //对话
            //输出
            //你好！有什么
            //var result = await chatgpt.GetChatMessageContentsAsync(historys,settings);
            //Console.WriteLine(result);

            //流式
            //输出
            //你好！有什么
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //对话————返回token
            //输出
            //你好！有什么
            //总消耗token：13 ,入参消耗token：10,出参消耗token：3
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");

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