using FaceMan.SemanticHub.KernelMemory;

namespace FaceMan.SemanticHub.Test.DataSet
{
    [TestClass]
    public class DataSetTest
    {
        [TestMethod]
        public async Task GetChatMessageContentsAsync()
        {
            var chatgpt = new KernelMemoryService();
            var _memory = chatgpt.GetMemoryByKMS();
            var text = "苏州市燃气管理办法 \r\n（2018 年 4月23日苏州市人民政府令第145号发布；https://minio.com           \t  \r\n2020 年 4 月21日苏州市人民政府令第153号第一次修正，  \r\n自2018年6月1日起施行） \r\n第一章  总  则 \r\n第一条  为了加强燃气管理，保障燃气供应，规范燃气\r\n的经营和使用，保障公民生命、财产安全和公共安全，维护\r\n燃气用户和燃气经营企业的合法权益，根据《中华人民共和\r\n国安全生产法》、国务院《城镇燃气管理条例》《江苏省燃\r\n气管理条例》等有关法律、法规，结合本市实际";
            await _memory.ImportTextAsync(text);
            var results = await _memory.SearchAsync("苏州市人民政府", "knowledgeBaseTest");
            //返回生成的文档摘要
            foreach (var item in results.Results)
            {
                Console.WriteLine($"== {item.SourceName} summary ==\n{item.Partitions.First().Text}\n");
            }
        }
    }
}
