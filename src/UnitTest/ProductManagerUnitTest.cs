using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solemart.BusinessLib;
using Solemart.SystemUtil;


namespace Solemart.UnitTest
{
    [TestClass]
    public class ProductManagerUnitTest
    {
        [TestInitialize]
        public void DbInitializeTest()
        {
            ConfigSettings.LoadAppConfig();
        }

        [TestMethod]
        public void TestGenerateProductImageFileName()
        {
            string filename = ProductManager.GenerateProductImageFileName(1, "ext");
            Assert.AreEqual<string>("1_1.ext", filename);
        }
    }
}
