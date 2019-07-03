using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Modules;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Modules
{
    [TestClass]
    public class ModuleManagerTests
    {
        [TestMethod]
        public void Register_And_Load()
        {

            var ModuleManager = new ModuleManager();

            ModuleManager.RegisterModule("MA", @"C:\Projects\vx1\MvvmLib\Samples\Modules\ModuleA\bin\Debug\ModuleA.dll", "ModuleA.ModuleAConfiguration");

            Assert.AreEqual(1, ModuleManager.Modules.Count);
            Assert.AreEqual("MA", ModuleManager.Modules["MA"].ModuleName);
            Assert.AreEqual(@"C:\Projects\vx1\MvvmLib\Samples\Modules\ModuleA\bin\Debug\ModuleA.dll", ModuleManager.Modules["MA"].Path);
            Assert.AreEqual("ModuleA.ModuleAConfiguration", ModuleManager.Modules["MA"].ModuleConfigurationFullName);
            Assert.AreEqual(false, ModuleManager.Modules["MA"].IsLoaded);
            Assert.AreEqual(0, SourceResolver.TypesForNavigation.Count);

            ModuleManager.LoadModule("MA");
            Assert.AreEqual(true, ModuleManager.Modules["MA"].IsLoaded);
            Assert.AreEqual(2, SourceResolver.TypesForNavigation.Count);
        }
    }
}
