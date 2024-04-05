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
  - 集成图像接口，仅支持通义万象，其他暂不对接，如有需要请提issue
  - 重构对话接口，采用Kernel的写法实现ITextGenerationService、IChatCompletionService、ITextToImageService，使其更灵活。
  - 更新单元测试
    # 使用方法
