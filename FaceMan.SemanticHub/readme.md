# SemanticHub
.Net ����SK���������ģ�͵�SDK������ʹ���߿��ٶԽӸ���ģ�ͣ�Ŀǰ��֧�ֶԻ�ģ��
- ֧����ʽ�ӿ�������ӿ�������ͳһ����κͳ��Σ�������á�

# Version
- v1.0.1 ���AzureOpenAI��OpenAI��ͨ��ǧ�ʡ�����AI��Ѷ���ǻ�����һ�ԶԻ�ģ�ͣ�֧�ִ���ģ�Ͳ�����
- v1.0.2 �Զ�������ַ�����ӵ�Ԫ���ԣ�ͳһAzureOpenAI��OpenAI������ģ�͵�д��

# ����ɶԻ�ģ��
 - [x] AzureOpenAI
 - [x] OpenAI
 - [x] ͨ��ǧ��
 - [x] Ѷ���ǻ�
 - [x] ����һ��
 - [x] ����AI
  
Ҳ����ֱ������Nuget��FaceMan.SemanticHub����
![image](https://github.com/faceman0814/SemanticHub/assets/74786133/c27744bb-cd4a-4ec2-9c75-9420d12c4c14)

# �ƻ�����
 - [ ] ͼ��ӿ�
 - [ ] ����ת���ֽӿ�
 - [ ] ����chatGLM��Gemini �ȸ����ģ��
 - [ ] δ�����������

 
# ʹ�÷���
```csharp
QianWenChatCompletionService chatgpt = new("���key", "�Ի�ģ�ͣ�����qwen-turbo");
ChatHistory historys = new ChatHistory();
historys.AddSystemMessage("����һ��c#��̸��֣��㽫�ô���ش��ҹ���.net��̵ļ������⣬�������ҵĵ�һ�����⣺");
historys.AddUserMessage("��c#дһ��ð������");

//����ģ�Ͳ���
 OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
 {
     MaxTokens = 1024,
     Temperature=0.7,
     TopP=1.0,
     //....��������
 };
//�Ի�
var result = await chatgpt.GetChatMessageContentsAsync(historys,settings);
Console.WriteLine(result);
//��ʽ�Ի�
await foreach (string item in chatgpt.GetStreamingChatMessageContentsAsync(historys,settings))
{
    Console.Write(item);
}
```

# ��ι���
�����ϣ�����빱�ף���ӭ Pull Requests,������� Issues
