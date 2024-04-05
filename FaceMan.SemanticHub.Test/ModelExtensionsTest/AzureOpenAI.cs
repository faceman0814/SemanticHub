using Azure.AI.OpenAI;

using FaceMan.SemanticHub;
using FaceMan.SemanticHub.Generation.ImageGeneration;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureChatCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureEmbeddingCompletion;
using FaceMan.SemanticHub.ModelExtensions.AzureOpenAI.AzureTextToImageCompletion;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace FaceMan.SemanticHub.Test.ModelExtensionsTest
{
    [TestClass]
    public class AzureOpenAI
    {
        private ChatHistory historys;
        private SemanticHubAzureOpenAIChatCompletionService chatgpt;
        private SemanticHubAzureOpenAIImageCompletionService imageService;
        private SemanticHubAzureOpenAIEmbeddingCompletionService textService;
        private OpenAIPromptExecutionSettings settings;

        public AzureOpenAI()
        {
            historys = new ChatHistory();
            //historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
            //historys.AddUserMessage("用c#写一个冒泡排序");

            string apiVersion = null;

            historys.AddUserMessage("你好");
            var input = new SemanticHubAzureOpenAIConfig()
            {
                ApiKey = "",
                Endpoint = "",
                DeploymentName = "",
                ApiVersion = apiVersion,
            };
            chatgpt = new(input);
            imageService = new(input);
            textService = new(input);
            settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 200,

                //....其他参数
            };
        }

        [TestMethod]
        public async Task GetChatMessageContentsAsync()
        {
            //对话
            //输出
            //你好！
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            const string FunctionDefinition = "Hi, give me 5 book suggestions about: {{$input}}";

            // Invoke function through kernel
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion("gpt-35-turbo", "https://faceman.openai.azure.com/", "b3d4d46e0e5847e19c690a58fe106fd9")
                .AddSemanticHubAzureOpenAIChatCompletion("gpt-35-turbo", "https://faceman.openai.azure.com/", "b3d4d46e0e5847e19c690a58fe106fd9")
                .Build();

            KernelFunction myFunction = kernel.CreateFunctionFromPrompt(FunctionDefinition);

            FunctionResult result = await kernel.InvokeAsync(myFunction, new() { ["input"] = "travel" });

            await foreach (var message in kernel.InvokeStreamingAsync(myFunction, new() { ["input"] = "travel" }))
            {
                Console.Write(message);
            }
            await foreach (var message in kernel.InvokeStreamingAsync<string>(myFunction, new() { ["input"] = "travel" }))
            {
                Console.Write(message);
            }
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
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"总消耗token：{resultToken.Item2.TotalTokens} ,入参消耗token：{resultToken.Item2.PromptTokens},出参消耗token：{resultToken.Item2.CompletionTokens}");
        }

        [TestMethod]
        public async Task GetStreamingChatMessageContentsByTokenAsync()
        {
            //流式————返回token
            //输出
            //你好，总消耗token：4 ,入参消耗token：2,出参消耗token：2
            var sum = new Usage();
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
        public async Task GetImageMessageContentsAsync()
        {
            var parameters = new ImageGenerationOptions()
            {
                Size = ImageSize.Size1024x1024
            };
            var result = await imageService.GenerateImageAsync("画一只小清新风格的鲸鱼", parameters);
            foreach (var item in result)
            {
                Console.WriteLine($"生成的ImgUrl：{item.ImageUrL}");
            }
        }

        [TestMethod]
        public async Task GetTextMessageContentsAsync()
        {
            var str = "\"text-embedding-ada-002\" 属于 OpenAI 大型语言模型系列的一部分。这个嵌入模型是为了将文本（如单词、短语或整段文本）转换为数值形式的向量，使得计算机能够处理和理解自然语言。下面用通俗的语言来解释它的几个主要特点：\r\n\r\n1. 理解文本的意义：这个模型不仅仅关注文本的字面意思，还能把握文本的深层含义。比如，它能理解同义词或上下文中词语的具体含义，使得生成的嵌入向量能反映这些语义特征。\r\n\r\n2. 高维数据压缩：它把复杂的文本信息压缩成较为简单的数值向量。想象一下，就像是把一长段话或者一个复杂的概念压缩成一个简单的数列，但这个数列依然保留了原本话语或概念的关键信息。\r\n\r\n3. 多用途：这种嵌入方式非常通用，适用于多种不同的自然语言处理任务，比如文本分类、情感分析、机器翻译等等。\r\n\r\n4. 基于深度学习：这个模型是基于深度学习技术构建的，这意味着它在大量文本数据上进行过训练，学会了如何有效地表示和理解语言。\r\n\r\n5. 转换为数值向量：将文本转换为数值向量的过程有助于计算机执行各种算法，因为计算机更擅长处理数字而不是文字。\r\n\r\n6. 可用于后续任务：一旦文本被转换成数值向量，这些向量就可以被用来训练机器学习模型，或者用于各种数据分析和自然语言处理任务。\r\n\r\n7. 预训练模型：\"text-embedding-ada-002\" 是一个预先训练好的模型，这意味着它已经在大量的文本数据上被训练过，所以你可以直接使用它而不需要从头开始训练。";
            var texts = await textService.GenerateEmbeddingsAsync(new List<string> { str });
            foreach (var item in texts)
            {
                Console.WriteLine(item);
            }
        }
    }


}
namespace Microsoft.SemanticKernel
{
    public static class SemanticHubAzureOpenAIChatCompletionTestClass
    {
        public static IKernelBuilder AddSemanticHubAzureOpenAIChatCompletion(
          this IKernelBuilder builder,
          string deploymentName,
          string endpoint,
          string apiKey)
        {

            builder.Services.AddKeyedSingleton<IChatCompletionService>("SemanticHubChatCompletionService", (serviceProvider, _) =>
            {
                var input = new SemanticHubAzureOpenAIConfig()
                {
                    ApiKey = apiKey,
                    Endpoint = endpoint,
                    DeploymentName = deploymentName,
                };
                return new SemanticHubAzureOpenAIChatCompletionService(input);
            });

            return builder;
        }
    }
}