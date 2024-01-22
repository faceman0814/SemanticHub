

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
            //historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            //historys.AddUserMessage("��c#дһ��ð������");
            historys.AddUserMessage("���");
            //OpenAIChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
            OpenAIChatCompletionService chatgpt = new("sk-YX7bGSR3hMivBqVq42F35a146b214f46AcCf4c4f073c2d49", "gpt-3.5-turbo", "https://oneapi.faceman.cn/v1");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....��������
            };

            ////�Ի�
            ////���
            ////��ã�
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            ////��ʽ
            ////���
            ////��ã�
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //�Ի�������������token
            //���
            //��ã�
            //������token��12 ,�������token��9,��������token��3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"������token��{resultToken.Item2.TotalTokens} ,�������token��{resultToken.Item2.PromptTokens},��������token��{resultToken.Item2.CompletionTokens}");

            //��ʽ������������token
            //���
            //��ã�������token��0 ,�������token��0,��������token��0��������OpenAI��ʽ�ӿڲ�����token�������
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
            //Console.Write($"������token��{sum.TotalTokens} ,�������token��{sum.PromptTokens},��������token��{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task AzureOpenAIChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            //historys.AddUserMessage("��c#дһ��ð������");
            historys.AddUserMessage("���");
            AzureOpenAIChatCompletionService chatgpt = new("28df2a037e7c432a85f7892d280b99c9", "https://yoyochatx.openai.azure.com/", "gpt-35-turbo");
            //AzureOpenAIChatCompletionService chatgpt = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:����Ĭ��2023-07-01-preview");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....��������
            };

            //�Ի�
            //���
            //��ã�
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //��ʽ
            //���
            //��ã�
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            ////�Ի�������������token
            ////���
            ////��ã�
            ////������token��12 ,�������token��9,��������token��3
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"������token��{resultToken.Item2.TotalTokens} ,�������token��{resultToken.Item2.PromptTokens},��������token��{resultToken.Item2.CompletionTokens}");

            //��ʽ������������token
            //���
            //��ã�������token��0 ,�������token��0,��������token��0
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
            Console.Write($"������token��{sum.TotalTokens} ,�������token��{sum.PromptTokens},��������token��{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task QianWenChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            //historys.AddUserMessage("��c#дһ��ð������");
            historys.AddUserMessage("���");
            //QianWenChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
            QianWenChatCompletionService chatgpt = new("sk-1e2853e50be14bba93f9a612aa71bb15", "qwen-turbo");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....��������
            };
            //�Ի�
            //���
            //��ã���ʲô
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //��ʽ
            //���
            //��ã���ʲô
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //�Ի�������������token
            //���
            //��ã���ʲô
            //������token��4 ,�������token��1,��������token��3
            var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            Console.WriteLine(resultToken.Item1);
            Console.Write($"������token��{resultToken.Item2.TotalTokens} ,�������token��{resultToken.Item2.PromptTokens},��������token��{resultToken.Item2.CompletionTokens}");

            //��ʽ������������token
            //���
            //��ã���ʲô������token��8 ,�������token��2,��������token��6
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
            //Console.Write($"������token��{sum.TotalTokens} ,�������token��{sum.PromptTokens},��������token��{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task WenXinChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            //historys.AddUserMessage("��c#дһ��ð������");
            historys.AddUserMessage("���");
            //WenXinChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
            WenXinChatCompletionService chatgpt = new("oSoGsTeMD1OGVaaInpnj3U9U", "PAkyqMhAq7S6IQKftjcAcUAn1PGOA2yU", "completions");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....��������
            };

            //�Ի�
            //���
            //��ã���ʲô
            //var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            //Console.WriteLine(result);

            //��ʽ
            //���
            //��ã���ʲô
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //�Ի�������������token
            //���
            //���ã�
            //������token��3 ,�������token��1,��������token��2
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"������token��{resultToken.Item2.TotalTokens} ,�������token��{resultToken.Item2.PromptTokens},��������token��{resultToken.Item2.CompletionTokens}");

            //��ʽ������������token
            //���
            //��ã�������token��4 ,�������token��2,��������token��2
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
            Console.Write($"������token��{sum.TotalTokens} ,�������token��{sum.PromptTokens},��������token��{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task XunFeiChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            //historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            //historys.AddUserMessage("��c#дһ��ð������");
            historys.AddUserMessage("���");
            //XunFeiChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourAppId", "YourModel:�ɲ��Ĭ��general", "YourEndPoint:�Զ�������ַ���ɲ���");

            XunFeiChatCompletionService chatgpt = new("a836acd63f6546265a843a3df0db1bc1", "NjBlYmNiOGMzNWQ3ODEwZGQ2NmY1MGJk", "1d29d888");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 3,
                //....��������
            };

            //�Ի�
            //���
            //��ã���ʲô
            //var result = await chatgpt.GetChatMessageContentsAsync(historys,settings);
            //Console.WriteLine(result);

            //��ʽ
            //���
            //��ã���ʲô
            //await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            //{
            //    Console.Write(item);
            //}

            //�Ի�������������token
            //���
            //��ã���ʲô
            //������token��13 ,�������token��10,��������token��3
            //var resultToken = await chatgpt.GetChatMessageContentsByTokenAsync(historys, settings);
            //Console.WriteLine(resultToken.Item1);
            //Console.Write($"������token��{resultToken.Item2.TotalTokens} ,�������token��{resultToken.Item2.PromptTokens},��������token��{resultToken.Item2.CompletionTokens}");

            //��ʽ������������token
            //���
            //��ã���ʲô������token��13 ,�������token��10,��������token��3
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
            Console.Write($"������token��{sum.TotalTokens} ,�������token��{sum.PromptTokens},��������token��{sum.CompletionTokens}");
        }

        [TestMethod]
        public async Task ZhiPuChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            ZhiPuChatCompletionService chatgpt = new("YourSecret", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                //....��������
            };

            //��ʽ
            await foreach (var item in chatgpt.GetStreamingChatMessageContentsAsync(historys, settings))
            {
                Console.Write(item);
            }
        }
    }
}