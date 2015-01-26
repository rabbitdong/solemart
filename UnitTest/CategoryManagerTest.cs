using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Xxx.EntityLib;
using Xxx.DataAccessLib;
using Xxx.BusinessLib;

namespace UnitTest {
    [TestClass]
    public class CategoryManagerTest {
        [TestInitialize]
        public void CreateDBConnection() {
            SystemConnection.ConnectString = "server=localhost;user id=root;pwd=;database=ILiveMini;";
        }

        [TestMethod]
        public void TestCategoies() {
            List<Category> categories = CategoryManager.Instance.Categories;
            Assert.AreEqual<int>(categories.Count, 2);
            Assert.AreEqual<string>(categories[0].CateName, "电子类");
            Assert.AreEqual<int>(categories[0].SubCategories.Count, 2);
            Assert.AreEqual<string>(categories[1].CateName, "Toy");
            Assert.AreEqual<int>(categories[1].SubCategories.Count, 1);
        }
    }
}
