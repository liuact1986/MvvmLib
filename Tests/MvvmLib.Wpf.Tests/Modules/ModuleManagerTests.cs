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
            ModuleManager.RegisterModule("MA", @"C:\Projects\vx1\MvvmLib\Samples\Modules\ModuleA\bin\Debug\ModuleA.dll", "ModuleA.ModuleAConfig");

            Assert.AreEqual(1, ModuleManager.Modules.Count);
            Assert.AreEqual("MA", ModuleManager.Modules["MA"].Name);
            Assert.AreEqual(@"C:\Projects\vx1\MvvmLib\Samples\Modules\ModuleA\bin\Debug\ModuleA.dll", ModuleManager.Modules["MA"].File);
            Assert.AreEqual("ModuleA.ModuleAConfig", ModuleManager.Modules["MA"].ModuleConfigFullName);
            Assert.AreEqual(false, ModuleManager.Modules["MA"].IsLoaded);
            Assert.AreEqual(0, SourceResolver.TypesForNavigation.Count);

            ModuleManager.LoadModule("MA");
            Assert.AreEqual(true, ModuleManager.Modules["MA"].IsLoaded);
            Assert.AreEqual(2, SourceResolver.TypesForNavigation.Count);

            ModuleManager.ClearModules();
        }
    }
}
