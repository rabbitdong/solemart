using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xxx.EntityLib;
using Xxx.DataAccessLib;
using Xxx.BusinessLib;

namespace UnitTest {
    [TestClass]
    public class ProductManagerTest {
        [TestInitialize]
        public void CreateDBConnection() {            
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ILiveMini;";
        }

        [TestMethod]
        public void TestGenerateProductImageFileName() {
            string file_name = ProductManager.Instance.GenerateProductImageFileName(1, "jpg");
            Assert.AreEqual<string>("1_3.jpg", file_name);

            file_name = ProductManager.Instance.GenerateProductImageFileName(2, "png");
            Assert.AreEqual<string>("2_0.png", file_name);
        }

        [TestMethod]
        public void TestFromMimeTypeGetExtendName() {
            string mime = "image/jpeg";
            string ext_name = ProductManager.Instance.FromMimeTypeGetExtendName(mime);
            Assert.AreEqual<string>("jpg", ext_name);

            mime = "image/png";
            Assert.AreEqual<string>("png", ProductManager.Instance.FromMimeTypeGetExtendName(mime));
        }
    }
}
