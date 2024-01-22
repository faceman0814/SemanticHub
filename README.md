
# SemanticHub

.Net 基于SK接入大语言模型的SDK，帮助使用者快速对接各大模型，目前仅支持对话模型

- 支持流式接口在聊天接口中整合统一的入参和出参，方便调用。

# Version

- v1.0.1 添加AzureOpenAI、OpenAI、通义千问、智谱AI、讯飞星火、文心一言对话模型，支持传入模型参数。
- v1.0.2 自定义代理地址，增加单元测试，统一AzureOpenAI、OpenAI与其他模型的写法
- v1.0.3 返回消耗的token数
- v1.0.4 开发中。。。。

  - [ ] 图像接口
  - [ ] 

# 已完成对话模型

- [X] AzureOpenAI
- [X] OpenAI
- [X] 通义千问
- [X] 讯飞星火
- [X] 文心一言
- [X] 智谱AI

也可以直接搜索Nuget包FaceMan.SemanticHub引入
![image](https://github.com/faceman0814/SemanticHub/assets/74786133/c27744bb-cd4a-4ec2-9c75-9420d12c4c14)

# 计划功能

- [X] 对话模型
- [X] 支持返回Token
- [ ] 图像接口
- [ ] 语音转文字接口
- [ ] 增加chatGLM、Gemini 等更多大模型
- [ ] 未完待续。。。

# 使用方法

```csharp
QianWenChatCompletionService chatgpt = new("你的key", "对话模型：例如qwen-turbo");
ChatHistory historys = new ChatHistory();
historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
historys.AddUserMessage("用c#写一个冒泡排序");

//创建模型参数
 OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
 {
     MaxTokens = 1024,
     Temperature=0.7,
     TopP=1.0,
     //....其他参数
 };
//对话
var result = await chatgpt.GetChatMessageContentsAsync(historys,settings);
Console.WriteLine(result);
//流式对话
await foreach (string item in chatgpt.GetStreamingChatMessageContentsAsync(historys,settings))
{
    Console.Write(item);
}
```

# 如何贡献

如果你希望参与贡献，欢迎 Pull Requests,或给我们 Issues
