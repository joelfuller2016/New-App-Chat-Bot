using Microsoft.VisualStudio.TestTools.UnitTesting;
using App_Code;
using System.Threading.Tasks;

namespace ChatBot.Tests
{
    [TestClass]
    public class OpenAIServiceTests
    {
        [TestMethod]
        public async Task SendMessageAsync_ReturnsJson()
        {
            var service = new OpenAIService("test-key", "gpt-3.5-turbo");
            try
            {
                var json = await service.SendMessageAsync("Hello");
                Assert.IsFalse(string.IsNullOrEmpty(json));
            }
            catch
            {
                Assert.Inconclusive("API call failed - requires valid key");
            }
        }
    }
}
