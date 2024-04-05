namespace FaceMan.SemanticHub.Test
{
    [TestClass]
    public class Example01
    {
        public Example01()
        {
        }

        [TestMethod]
        public async Task GetChatMessageContentsAsync()
        {
            //对话
            //输出
            //你好
            var result = await chatgpt.GetChatMessageContentsAsync(historys, settings);
            Console.WriteLine(result[0].Content);
        }
    }
}
