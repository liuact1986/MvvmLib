using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using MvvmLib.Adaptive;

namespace MvvmLib.Adaptive.Win.Tests
{
    [TestClass]
    public class AdaptiveJsonFileServiceTest
    {
        [TestMethod]
        public async Task TestReadFileAsync()
        {
            var service = new AdaptiveJsonFileService();
            var json = await service.ReadFileAsync("Common/test.json");
            Assert.AreEqual("{\"test\":\"ok\"}", json);
        }

        [TestMethod]
        public async Task TestLoadAsync()
        {
            var service = new AdaptiveJsonFileService();
            var result = await service.LoadAsync("Common/bind.json");
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, result[0].minwidth);
            Assert.AreEqual("20", result[0].bindings["TitleFontSize"]);
            Assert.AreEqual("red", result[0].bindings["TitleColor"]);
            Assert.AreEqual(500, result[1].minwidth);
            Assert.AreEqual("50", result[1].bindings["TitleFontSize"]);
            Assert.AreEqual("blue", result[1].bindings["TitleColor"]);
            Assert.AreEqual(1000, result[2].minwidth);
            Assert.AreEqual("100", result[2].bindings["TitleFontSize"]);
            Assert.AreEqual("green", result[2].bindings["TitleColor"]);
        }

        [TestMethod]
        public async Task TestCache()
        {
            var service = new AdaptiveJsonFileService();
            var file = "Common/bind.json";

            Assert.IsFalse(service.IsCached(file));

            await service.LoadAsync(file);

            Assert.IsTrue(service.IsCached(file));

            var result = service.GetFromCache(file);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, result[0].minwidth);
            Assert.AreEqual("20", result[0].bindings["TitleFontSize"]);
            Assert.AreEqual("red", result[0].bindings["TitleColor"]);
            Assert.AreEqual(500, result[1].minwidth);
            Assert.AreEqual("50", result[1].bindings["TitleFontSize"]);
            Assert.AreEqual("blue", result[1].bindings["TitleColor"]);
            Assert.AreEqual(1000, result[2].minwidth);
            Assert.AreEqual("100", result[2].bindings["TitleFontSize"]);
            Assert.AreEqual("green", result[2].bindings["TitleColor"]);
        }
    }
}
