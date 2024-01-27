# SemanticHub

.Net 基于SK接入大语言模型的SDK，帮助使用者快速对接各大模型。

# 计划功能

- [X] 对话模型

  - [X] AzureOpenAI
  - [X] OpenAI
  - [X] 通义千问
  - [X] 讯飞星火
  - [X] 文心一言
  - [X] 智谱AI
- [X] 支持返回Token
- [X] 支持自定义代理地址
- [X] 图像接口

  - [X] Azure
  - [X] 通义万象
- [ ] 文本处理接口
- [ ] 语音转文字接口
- [ ] 增加chatGLM、Gemini 等更多大模型
- [ ] 未完待续。。。

# Version

- v1.0.1
  - 添加AzureOpenAI、OpenAI、通义千问、智谱AI、讯飞星火、文心一言对话模型，支持传入模型参数。
- v1.0.2
  - 自定义代理地址
  - 增加单元测试
  - 统一AzureOpenAI、OpenAI与其他模型的写法
- v1.0.3
  - 返回消耗的token数
- v1.0.4
  - 集成图像接口，仅支持Azure，通义万象，其他暂不对接，如有需要请提issue
  - 开发中。。。。

也可以直接搜索Nuget包FaceMan.SemanticHub引入
![image](https://github.com/faceman0814/SemanticHub/assets/74786133/c27744bb-cd4a-4ec2-9c75-9420d12c4c14)

# 使用方法

项目地址：[faceman0814/SemanticHub (github.com)](https://github.com/faceman0814/SemanticHub)

参考单元测试内容

# Tips

- 单元测试时的时候不要并发点，会报错，因为他是websocket访问。
- OpenAI 采用流式方法对话时，不会返回消耗的token。
- 各模型厂商对token的消耗计算是不一样的，可以通过单元测试对比结果。
- 测试中通义千问流式方法消耗的token比直接对话的要多一点。
- 文心一言实际调用中的模型并不叫模型名，要跟官方的对标好比如ernie-bot实际在代码中的传参是completions，这个后续优化。
- 文心一言有概率乱答

# 如何贡献

如果你希望参与贡献，欢迎 Pull Requests,或给我们 Issues
