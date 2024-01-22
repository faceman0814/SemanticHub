

using FaceMan.SemanticHub.ModelExtensions;
using FaceMan.SemanticHub.ModelExtensions.OpenAI;
using FaceMan.SemanticHub.ModelExtensions.QianWen;
using FaceMan.SemanticHub.ModelExtensions.WenXin;
using FaceMan.SemanticHub.ModelExtensions.XunFei;
using FaceMan.SemanticHub.ModelExtensions.ZhiPu;

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
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            OpenAIChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:����Ĭ��https://api.openai.com/v1");
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

        [TestMethod]
        public async Task AzureOpenAIChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            AzureOpenAIChatCompletionService chatgpt = new("YourKey", "YourEndPoint", "YourDeploymentName", "YourApiVersion:����Ĭ��2023-07-01-preview");
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

        [TestMethod]
        public async Task QianWenChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            QianWenChatCompletionService chatgpt = new("YourKey", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
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

        [TestMethod]
        public async Task WenXinChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            WenXinChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
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

        [TestMethod]
        public async Task XunFeiChatMessageContentTest()
        {
            ChatHistory historys = new ChatHistory();
            historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
            historys.AddUserMessage("��c#дһ��ð������");
            XunFeiChatCompletionService chatgpt = new("YourKey", "YourSecret", "YourAppId", "YourModel", "YourEndPoint:�Զ�������ַ���ɲ���");
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