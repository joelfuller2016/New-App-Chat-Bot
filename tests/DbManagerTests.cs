using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using App_Code;

namespace ChatBot.Tests
{
    [TestClass]
    public class DbManagerTests
    {
        [TestMethod]
        public void Initialize_CreatesDatabase()
        {
            DbManager.Initialize();
            var path = Path.Combine(System.AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "chatbot.db");
            Assert.IsTrue(File.Exists(path));
        }
    }
}
