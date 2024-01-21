# SemanticHub
.Net 基于SK接入大语言模型的SDK，帮助使用者快速对接各大模型，目前仅支持对话模型，支持流式接口在聊天接口中整合统一的入参和出参。方便调用。

# 已完成对话模型
 - [x] AzureOpenAI
 - [x] OpenAI
 - [x] 通义千问
 - [x] 讯飞星火
 - [x] 文心一言
 - [x] 智谱AI
  
也可以直接搜索Nuget包FaceMan.SemanticHub引入
![image](https://github.com/faceman0814/SemanticHub/assets/74786133/c27744bb-cd4a-4ec2-9c75-9420d12c4c14)

# 计划功能
 - [ ] 图像接口
 - [ ] 语音转文字接口
 - [ ] 增加chatGLM、Gemini 等更多大模型
 - [ ] 未完待续
# Version
- v1.0.1 添加AzureOpenAI、OpenAI、通义千问、智谱AI、讯飞星火、文心一言对话模型。

# 使用方法
```
QianWenChatCompletionService chatgpt = new("你的key", "对话模型：例如qwen-turbo");
ChatHistory historys = new ChatHistory();
historys.AddSystemMessage("你是一个c#编程高手，你将用代码回答我关于.net编程的技术问题，下面是我的第一个问题：");
historys.AddUserMessage("用c#写一个冒泡排序");

//对话
var result = await chatgpt.GetChatMessageContentsAsync(historys);
Console.WriteLine(result);
//流式对话
await foreach (string item in chatgpt.GetStreamingChatMessageContentsAsync(historys))
{
    Console.Write(item);
}
```

# 如何贡献
如果你希望参与贡献，欢迎 Pull Requests,或给我们 Issues
